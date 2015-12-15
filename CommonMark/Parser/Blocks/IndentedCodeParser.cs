using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.IndentedCode"/> element parser.
    /// </summary>
    public class IndentedCodeParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedCodeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public IndentedCodeParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get { return null; }
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
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            if (info.IsIndented)
            {
                info.AdvanceIndentedOffset();
                return true;
            }
            if (info.IsBlank)
            {
                info.AdvanceOffset(info.FirstNonspace - info.Offset, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            if (info.IsIndented && !info.IsMaybeLazy && !info.IsBlank)
            {
                info.AdvanceIndentedOffset();
                info.Container = CreateChildBlock(info, BlockTag.IndentedCode, info.Offset);
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
            return true;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            container.StringContent.RemoveTrailingBlankLines();
            return true;
        }
    }
}
