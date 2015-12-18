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
            : base(settings, BlockTag.IndentedCode)
        {
            IsCodeBlock = true;
            IsAcceptsLines = true;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
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
                info.Container = CreateChildBlock(info, Tag, info.Offset);
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
