using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// HTML block delimiter handler.
    /// </summary>
    public sealed class HtmlBlockHandler : BlockDelimiterHandler
    {
        private static readonly string[] blockTagNames = new[] { "ADDRESS", "ARTICLE", "ASIDE", "BASE", "BASEFONT", "BLOCKQUOTE", "BODY", "CAPTION", "CENTER", "COL", "COLGROUP", "DD", "DETAILS", "DIALOG", "DIR", "DIV", "DL", "DT", "FIELDSET", "FIGCAPTION", "FIGURE", "FOOTER", "FORM", "FRAME", "FRAMESET", "H1", "HEAD", "HEADER", "HR", "HTML", "IFRAME", "LEGEND", "LI", "MAIN", "MENU", "MENUITEM", "META", "NAV", "NOFRAMES", "OL", "OPTGROUP", "OPTION", "P", "PARAM", "PRE", "SCRIPT", "SECTION", "SOURCE", "STYLE", "SUMMARY", "TABLE", "TBODY", "TD", "TFOOT", "TH", "THEAD", "TITLE", "TR", "TRACK", "UL" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlockHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        public HtmlBlockHandler(CommonMarkSettings settings, BlockTag tag)
            : base(settings, tag, '<')
        {
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            HtmlBlockType htmlBlockType;
            if (!info.IsIndented && (0 != (htmlBlockType = ScanStart(info))
                || (info.Container.Tag != BlockTag.Paragraph && 0 != (htmlBlockType = ScanStart2(info)))))
            {
                info.Container = AppendChildBlock(info, Tag, info.FirstNonspace);
                info.Container.HtmlBlockType = htmlBlockType;
                // note, we don't adjust offset because the tag is part of the text
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to match an HTML block start.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>Block type, or <see cref="HtmlBlockType.None"/> on no match.</returns>
        /// <remarks>Original: HtmlBlockType scan_html_block_start(string s, int pos, int sourceLength)</remarks>
        private static HtmlBlockType ScanStart(BlockParserInfo info)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            if (pos + 2 >= sourceLength)
                return HtmlBlockType.None;

            if (s[pos] != '<')
                return HtmlBlockType.None;

            var c = s[++pos];
            if (c == '!')
            {
                c = pos + 2 >= sourceLength ? '\0' : s[++pos];
                if (c >= 'A' && c <= 'Z')
                    return HtmlBlockType.DocumentType;

                if (c == '[' && pos + 7 < sourceLength
                    && string.Equals("CDATA[", s.Substring(pos + 1, 6), StringComparison.Ordinal))
                    return HtmlBlockType.CData;

                if (c == '-' && pos + 1 < sourceLength && s[pos + 1] == '-')
                    return HtmlBlockType.Comment;

                return HtmlBlockType.None;
            }

            if (c == '?')
            {
                return HtmlBlockType.ProcessingInstruction;
            }

            var slashAtBeginning = c == '/';
            if (slashAtBeginning)
                c = s[++pos];

            var j = 0;
            var tagname = new char[10];
            while (((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '1' && c <= '6')) && j < 10 && ++pos < sourceLength)
            {
                tagname[j++] = c;
                c = s[pos];
            }

            if (c != '>' && (c != '/' || pos + 1 >= sourceLength || s[pos + 1] != '>') && !Utilities.IsWhitespace(c))
                return HtmlBlockType.None;

            var tname = new string(tagname, 0, j).ToUpperInvariant();
            var tagIndex = Array.BinarySearch(blockTagNames, tname, StringComparer.Ordinal);
            if (tagIndex < 0)
                return HtmlBlockType.None;

            if (tagIndex == 44 || tagIndex == 45 || tagIndex == 48)
                return c == '/' ? HtmlBlockType.None : HtmlBlockType.InterruptingBlockWithEmptyLines;

            return HtmlBlockType.InterruptingBlock;
        }

        /// <summary>
        /// Attempts to match an HTML block start.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>Block type, or <see cref="HtmlBlockType.None"/> on no match.</returns>
        /// <remarks>Original: HtmlBlockType scan_html_block_start_7(string s, int pos, int sourceLength)</remarks>
        private static HtmlBlockType ScanStart2(BlockParserInfo info)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            if (pos + 2 >= sourceLength)
                return HtmlBlockType.None;

            if (s[pos] != '<')
                return HtmlBlockType.None;

            int i = pos;
            if (s[++i] == '/')
                i = ElementParser.ScanHtmlCloseTag(s, i, sourceLength);
            else
                i = ElementParser.ScanHtmlOpenTag(s, i, sourceLength);

            if (i == 0)
                return HtmlBlockType.None;

            i += pos;
            while (++i < sourceLength)
            {
                if (!Utilities.IsWhitespace(s[i]))
                    return HtmlBlockType.None;
            }

            return HtmlBlockType.NonInterruptingBlock;
        }
    }
}
