using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.FencedCode"/> element parser.
    /// </summary>
    public class FencedCodeParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public FencedCodeParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get
            {
                return new[] { '`', '~' };
            }
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
        /// Gets the value indicating whether a handled element accepts new lines.
        /// </summary>
        /// <value><c>true</c> if new lines can be added to a handled element.</value>
        public override bool IsAcceptsLines
        {
            get { return true; }
        }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if blank lines at the end of the handled element should be discarded.</returns>
        public override bool IsDiscardLastBlank(BlockParserInfo info)
        {
            // we don't count blanks in fenced code for purposes of tight/loose lists or breaking out of lists.
            return true;
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            // -1 means we've seen closer 
            if (info.Container.FencedCodeData.FenceLength == -1)
            {
                if (info.IsBlank)
                    info.Container.IsLastLineBlank = true;
                return false;
            }

            // skip optional spaces of fence offset
            var i = info.Container.FencedCodeData.FenceOffset;
            while (i > 0 && info.Line[info.Offset] == ' ')
            {
                info.Offset++;
                info.Column++;
                i--;
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
            int matched;
            if (!info.IsIndented && 0 != (matched = Scanner.scan_open_code_fence(info.Line, info.FirstNonspace, info.Line.Length, Settings.BlockParserParameters)))
            {
                info.Container = CreateChildBlock(info, BlockTag.FencedCode, info.FirstNonspace);
                info.Container.FencedCodeData = new FencedCodeData
                {
                    FenceChar = info.CurrentCharacter,
                    FenceLength = matched,
                    FenceOffset = info.FirstNonspace - info.Offset,
                };
                info.AdvanceOffset(info.FirstNonspace + matched - info.Offset, false);
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
            if (!info.IsIndented && info.CurrentCharacter == info.Container.FencedCodeData.FenceChar
              && (0 != Scanner.scan_close_code_fence(info.Line, info.FirstNonspace, info.Container.FencedCodeData.FenceLength, info.Line.Length, Settings.BlockParserParameters)))
            {
                // if closing fence, set fence length to -1. it will be closed when the next line is processed. 
                info.Container.FencedCodeData.FenceLength = -1;
            }
            else
            {
                AddLine(info.Container, info.LineInfo, info.Line, info.Offset);
            }
            return true;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            // first line of contents becomes info
            var firstlinelen = container.StringContent.IndexOf('\n') + 1;
            container.FencedCodeData.Info = InlineMethods.Unescape(container.StringContent.TakeFromStart(firstlinelen, true).Trim());
            return true;
        }
    }
}
