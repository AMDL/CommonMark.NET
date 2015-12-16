using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Automatic email link parser.
    /// </summary>
    public class AutoEmailParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoEmailParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public AutoEmailParser(CommonMarkSettings settings)
            : base(settings, '<')
        {
        }

        /// <summary>
        /// Attempts to match an email autolink.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        public override Inline Handle(Block container, Subject subj)
        {
            // advance past first <
            subj.Position++;

            var matchlen = Scan(subj.Buffer, subj.Position, subj.Length);
            if (matchlen == 0)
            {
                subj.Position--;
                return null;
            }

            var contents = subj.Buffer.Substring(subj.Position, matchlen - 1);
            var resultContents = ParseStringEntities(contents);
            return CreateLink(subj, resultContents, "mailto:" + contents, string.Empty, matchlen);
        }

        /// <summary>
        /// Try to match email autolink after first &lt;, returning num of chars matched.
        /// </summary>
        /// <remarks>Original: int scan_autolink_email(string s, int pos, int sourceLength)</remarks>
        private static int Scan(string s, int pos, int sourceLength)
        {
            /*!re2c
              [a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+
                [@]
                [a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
                ([.][a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*
                [>] { return (p - start); }
              .? { return 0; }
            */

            if (pos + 6 >= sourceLength)
                return 0;

            char c = s[pos];
            if (c == '@')
                return 0;

            int i = pos;
            int ln = sourceLength - 1;
            while (i <= ln)
            {
                if (c == '@')
                    break;

                if ((c < 'a' || c > 'z')
                    && (c < 'A' || c > 'Z')
                    && (c < '0' || c > '9')
                    && ".!#$%&'*+/=?^_`{|}~-".IndexOf(c) == -1)
                    return 0;
                if (i == ln) return 0;
                c = s[++i];
            }

            // move past '@'
            if (i == ln) return 0;
            c = s[++i];
            bool hadDot = false;

            while (true)
            {
                var domainStart = i;
                if (!ScannerCharacterMatcher.MatchAsciiLetterOrDigit(s, ref c, ref i, ln, '-'))
                    return 0;

                if (s[i - 1] == '-' || i - domainStart > 63)
                    return 0;

                if (c == '>')
                    return hadDot ? i - pos + 1 : 0;

                if (c != '.' || i == ln)
                    return 0;

                hadDot = true;
                c = s[++i];
            }
        }
    }
}
