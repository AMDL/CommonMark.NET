using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Indented code delimiter handler.
    /// </summary>
    public sealed class IndentedCodeHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedCodeHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        public IndentedCodeHandler(CommonMarkSettings settings, BlockTag tag)
            : base(settings, tag)
        {
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            if (info.IsIndented && !info.IsMaybeLazy && !info.IsBlank)
            {
                info.AdvanceIndentedOffset();
                info.Container = AppendChildBlock(info, Tag, info.Offset);
            }
            return false;
        }
    }
}
