using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.HtmlBlock"/> element parser.
    /// </summary>
    public sealed class HtmlBlockParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlockParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public HtmlBlockParser(CommonMarkSettings settings)
            : base(settings, BlockTag.HtmlBlock)
        {
            IsCodeBlock = true;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                yield return new Delimiters.HtmlBlockHandler(Settings, Tag);
            }
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
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
        /// Attempts to match an HTML block end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        /// <remarks>Original: bool scan_html_block_end(HtmlBlockType type, string s, int pos, int sourceLength)</remarks>
        private bool ScanEnd(BlockParserInfo info)
        {
            switch (info.Container.HtmlBlockType)
            {
                case HtmlBlockType.InterruptingBlockWithEmptyLines:
                    return ScanEndInterruptingBlockWithEmptyLines(info);
                case HtmlBlockType.Comment:
                    return ScanEndComment(info);
                case HtmlBlockType.ProcessingInstruction:
                    return ScanEndProcessingInstruction(info);
                case HtmlBlockType.DocumentType:
                    return ScanEndDocumentType(info);
                case HtmlBlockType.CData:
                    return ScanEndCData(info);
                default:
                    return false;
            }
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
