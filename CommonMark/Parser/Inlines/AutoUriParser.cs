using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Automatic URI link parser.
    /// </summary>
    public class AutoUriParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUriParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public AutoUriParser(CommonMarkSettings settings)
            : base(settings, '<')
        {
        }

        /// <summary>
        /// Attempts to match a URL autolink.
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
            return CreateLink(subj, resultContents, contents, string.Empty, matchlen);
        }

        /// <summary>
        /// Try to match URI autolink after first &lt;, returning number of chars matched.
        /// </summary>
        /// <remarks>Original: int scan_autolink_uri(string s, int pos, int sourceLength)</remarks>
        private static int Scan(string s, int pos, int sourceLength)
        {
            // for now the tests do not include anything that would require the use of `escaped_char` part so it is ignored.

            // 31 is the maximum length of a valid scheme
            var checkLen = sourceLength - pos;
            if (checkLen > 31)
                checkLen = 31;

            var colon = false;
            for (var i = pos; i < sourceLength; i++)
            {
                var c = s[i];
                switch (c)
                {
                    case ':':
                        if (i < pos + 2 || i >= checkLen)
                            return 0;
                        colon = true;
                        break;

                    case '>':
                        if (!colon)
                            return 0;
                        return i - pos + 1;

                    case '<':
                        return 0;

                    default:
                        if (i == pos)
                        {
                            if (!(c >= 'A' && c <= 'Z') && !(c >= 'a' && c <= 'z') && c != ' ')
                                return 0;
                            break;
                        }
                        if (!colon)
                        {
                            if (!(c >= 'A' && c <= 'Z') && !(c >= 'a' && c <= 'z') && c != '.' && c != '+' && c != '-')
                                return 0;
                            break;
                        }
                        if (c <= 0x20)
                            return 0;
                        break;
                }
            }

            return 0;
        }
    }
}
