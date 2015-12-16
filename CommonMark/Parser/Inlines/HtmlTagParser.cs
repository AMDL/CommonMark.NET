using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// HTML tag parser.
    /// </summary>
    public class HtmlTagParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTagParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public HtmlTagParser(CommonMarkSettings settings)
            : base('<', settings)
        {
        }

        /// <summary>
        /// Attempts to match an html tag.
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

            var result = new Inline(InlineTag.RawHtml, subj.Buffer, subj.Position - 1, matchlen + 1)
            {
                SourcePosition = subj.Position - 1,
                SourceLastPosition = subj.Position + matchlen,
            };

            subj.Position += matchlen;
            return result;
        }

        /// <summary>
        /// Attempts to match an HTML tag after first &lt;.
        /// </summary>
        /// <returns>Number of chars matched, or 0 for no match.</returns>
        /// <remarks>Original: int scan_html_tag(string s, int pos, int sourceLength)</remarks>
        private static int Scan(string s, int pos, int sourceLength)
        {
            if (pos + 2 >= sourceLength)
                return 0;

            var firstChar = s[pos];

            if (firstChar == '/')
                return ScanHtmlCloseTag(s, pos, sourceLength);

            if (firstChar == '?')
                return ScanProcessingInstruction(s, pos, sourceLength);

            if (firstChar == '!')
            {
                var nextChar = s[pos + 1];
                if (nextChar == '-')
                    return ScanHtmlComment(s, pos, sourceLength);

                if (nextChar == '[')
                    return ScanCData(s, pos, sourceLength);

                return ScanDeclaration(s, pos, sourceLength);
            }

            return ScanHtmlOpenTag(s, pos, sourceLength);
        }

        /// <summary>
        /// Attempts to match an HTML processing instruction.
        /// </summary>
        /// <remarks>Original: int _scanHtmlTagProcessingInstruction(string s, int pos, int sourceLength)</remarks>
        private static int ScanProcessingInstruction(string s, int pos, int sourceLength)
        {
            // Original regexp: "\\?(([^?>\\x00]+)|([?][^>\\x00]))*\\?>"
            // note the original regexp is invalid since it does not allow '>' within the tag.

            char nextChar;
            char lastChar = '\0';
            for (var i = pos + 1; i < sourceLength; i++)
            {
                nextChar = s[i];

                if (nextChar == '>' && lastChar == '?')
                    return i - pos + 1;

                lastChar = nextChar;
            }

            return 0;
        }

        /// <summary>
        /// Attempts to match an HTML comment.
        /// </summary>
        /// <remarks>Original: int _scanHtmlTagHtmlComment(string s, int pos, int sourceLength)</remarks>
        private static int ScanHtmlComment(string s, int pos, int sourceLength)
        {
            // we know that the initial "!-" has already been verified
            if (pos + 5 >= sourceLength)
                return 0;

            if (s[pos + 2] != '-')
                return 0;

            char nextChar = s[pos + 3];
            if (nextChar == '>' || (nextChar == '-' && s[pos + 4] == '>'))
                return 0;

            byte hyphenCount = 0;
            for (var i = pos + 3; i < sourceLength; i++)
            {
                nextChar = s[i];

                if (hyphenCount == 2)
                    return nextChar == '>' ? i - pos + 1 : 0;

                if (nextChar == '-')
                    hyphenCount++;
                else
                    hyphenCount = 0;
            }

            return 0;
        }

        /// <summary>
        /// Attempts to match an HTML CDATA element.
        /// </summary>
        /// <remarks>Original: int _scanHtmlTagCData(string s, int pos, int sourceLength)</remarks>
        private static int ScanCData(string s, int pos, int sourceLength)
        {
            // Original regexp: "\\!\\[CDATA\\[(([^\\]\\x00]+)|(\\][^\\]\\x00])|(\\]\\][^>\\x00]))*\\]\\]>"

            if (pos + 10 >= sourceLength)
                return 0;

            if (!string.Equals(s.Substring(pos, 8), "![CDATA[", StringComparison.Ordinal))
                return 0;

            var bracketCount = 0;
            char nextChar;
            for (var i = pos + 8; i < sourceLength; i++)
            {
                nextChar = s[i];

                if (nextChar == '>' && bracketCount >= 2)
                    return i - pos + 1;

                if (nextChar == ']')
                    bracketCount++;
                else
                    bracketCount = 0;
            }

            return 0;
        }

        /// <summary>
        /// Attempts to match an HTML element.
        /// </summary>
        /// <remarks>Original: int _scanHtmlTagDeclaration(string s, int pos, int sourceLength)</remarks>
        private static int ScanDeclaration(string s, int pos, int sourceLength)
        {
            // Original regexp: "\\![A-Z]+\\s+[^>\\x00]*>"

            // minimum value: "!A >"
            if (pos + 4 >= sourceLength)
                return 0;

            var spaceFound = false;
            char nextChar = s[pos + 2];
            if (nextChar < 'A' || nextChar > 'Z')
                return 0;

            for (var i = pos + 3; i < sourceLength; i++)
            {
                nextChar = s[i];

                if (nextChar == '>')
                    return spaceFound ? i - pos + 1 : 0;

                if (nextChar == ' ' || nextChar == '\n')
                    spaceFound = true;
            }

            return 0;
        }
    }
}
