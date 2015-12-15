using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.HtmlBlock"/> element parser.
    /// </summary>
    public class HtmlBlockParser : BlockParser
    {
        private static readonly string[] blockTagNames = new[] { "ADDRESS", "ARTICLE", "ASIDE", "BASE", "BASEFONT", "BLOCKQUOTE", "BODY", "CAPTION", "CENTER", "COL", "COLGROUP", "DD", "DETAILS", "DIALOG", "DIR", "DIV", "DL", "DT", "FIELDSET", "FIGCAPTION", "FIGURE", "FOOTER", "FORM", "FRAME", "FRAMESET", "H1", "HEAD", "HEADER", "HR", "HTML", "IFRAME", "LEGEND", "LI", "MAIN", "MENU", "MENUITEM", "META", "NAV", "NOFRAMES", "OL", "OPTGROUP", "OPTION", "P", "PARAM", "PRE", "SCRIPT", "SECTION", "SOURCE", "STYLE", "SUMMARY", "TABLE", "TBODY", "TD", "TFOOT", "TH", "THEAD", "TITLE", "TR", "TRACK", "UL" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlockParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public HtmlBlockParser(CommonMarkSettings settings)
            : base(settings)
        {
            _endScanners = settings.GetLazy(InitializeEndScanners);
        }

        /// <summary>
        /// Gets the value indicating whether a handled element is a code block.
        /// </summary>
        /// <value><c>true</c> if a handled element is a code block.</value>
        public override bool IsCodeBlock
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get
            {
                return new[] { '<' };
            }
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            // all other block types can accept blanks
            if (info.IsBlank && info.Container.HtmlBlockType >= HtmlBlockType.InterruptingBlock)
            {
                info.Container.IsLastLineBlank = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            HtmlBlockType htmlBlockType;
            if (!info.IsIndented && (0 != (htmlBlockType = ScanStart(info))
                || (info.Container.Tag != BlockTag.Paragraph && 0 != (htmlBlockType = ScanStart2(info)))))
            {
                info.Container = CreateChildBlock(info, BlockTag.HtmlBlock, info.FirstNonspace);
                info.Container.HtmlBlockType = htmlBlockType;
                // note, we don't adjust offset because the tag is part of the text
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Close(BlockParserInfo info)
        {
            AddLine(info.Container, info.LineInfo, info.Line, info.Offset);

            if (ScanEnd(info))
            {
                BlockMethods.Finalize(info.Container, info.LineInfo, Settings);
                info.Container = info.Container.Parent;
            }

            return true;
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
                i = ScanHtmlCloseTag(s, i, sourceLength);
            else
                i = ScanHtmlOpenTag(s, i, sourceLength);

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

        /// <summary>
        /// Attempts to match an HTML block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool scan_html_block_end(HtmlBlockType type, string s, int pos, int sourceLength)</remarks>
        private bool ScanEnd(BlockParserInfo info)
        {
            var scanner = EndScanners[(int)info.Container.HtmlBlockType];
            return scanner != null && scanner(info);
        }

        private Lazy<HtmlBlockCloserDelegate[]> _endScanners;

        private HtmlBlockCloserDelegate[] EndScanners
        {
            get { return _endScanners.Value; }
        }

        private static HtmlBlockCloserDelegate[] InitializeEndScanners()
        {
            var s = new HtmlBlockCloserDelegate[(int)HtmlBlockType.Count];
            s[(int)HtmlBlockType.InterruptingBlockWithEmptyLines] = ScanEndInterruptingBlockWithEmptyLines;
            s[(int)HtmlBlockType.Comment] = ScanEndComment;
            s[(int)HtmlBlockType.ProcessingInstruction] = ScanEndProcessingInstruction;
            s[(int)HtmlBlockType.DocumentType] = ScanEndDocumentType;
            s[(int)HtmlBlockType.CData] = ScanEndCData;
            return s;
        }

        /// <summary>
        /// Attempts to match an HTML interrupting block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool _scan_html_block_end_1(string s, int pos, int sourceLength)</remarks>
        private static bool ScanEndInterruptingBlockWithEmptyLines(BlockParserInfo info)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            //  .* [<] [/] ('script'|'pre'|'style') [>] { return (bufsize_t)(p - start); }
            var i = pos;

            while (i + 5 < sourceLength)
            {
                i = s.IndexOf('<', i, sourceLength - i - 3);
                if (i == -1)
                    break;

                if (s[++i] != '/')
                    continue;

                var c = s[++i];
                if (c != 's' && c != 'S' && c != 'p' && c != 'P')
                    continue;

                var j = s.IndexOf('>', i, Math.Min(sourceLength - i, 7));
                if (j == -1)
                    continue;

                var t = s.Substring(i, j - i).ToUpperInvariant();
                if (string.Equals("PRE", t, StringComparison.Ordinal)
                    || string.Equals("STYLE", t, StringComparison.Ordinal)
                    || string.Equals("SCRIPT", t, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to match an HTML comment block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool _scan_html_block_end_2(string s, int pos, int sourceLength)</remarks>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool ScanEndComment(BlockParserInfo info)
        {
            //  .* '-->' { return (bufsize_t)(p - start); }

            var i = info.Line.IndexOf("-->", info.FirstNonspace, info.NonspaceLength, StringComparison.Ordinal);
            return i > -1;
        }

        /// <summary>
        /// Attempts to match an HTML processing instruction block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool _scan_html_block_end_3(string s, int pos, int sourceLength)</remarks>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool ScanEndProcessingInstruction(BlockParserInfo info)
        {
            //  .* '?>' { return (bufsize_t)(p - start); }

            var i = info.Line.IndexOf("?>", info.FirstNonspace, info.NonspaceLength, StringComparison.Ordinal);
            return i > -1;
        }

        /// <summary>
        /// Attempts to match an HTML doctype block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool _scan_html_block_end_4(string s, int pos, int sourceLength)</remarks>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool ScanEndDocumentType(BlockParserInfo info)
        {
            //  .* '>' { return (bufsize_t)(p - start); }
            var i = info.Line.IndexOf('>', info.FirstNonspace, info.NonspaceLength);
            return i > -1;
        }

        /// <summary>
        /// Attempts to match an HTML CDATA block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool _scan_html_block_end_5(string s, int pos, int sourceLength)</remarks>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool ScanEndCData(BlockParserInfo info)
        {
            //  .* ']]>' { return (bufsize_t)(p - start); }
            var i = info.Line.IndexOf("]]>", info.FirstNonspace, info.NonspaceLength, StringComparison.Ordinal);
            return i > -1;

        }
    }
}
