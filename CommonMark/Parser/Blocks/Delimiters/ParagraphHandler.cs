using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Paragraph delimiter handler.
    /// </summary>
    public sealed class ParagraphHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        public ParagraphHandler(CommonMarkSettings settings, BlockTag tag)
            : base(settings, tag, '\0')
        {
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            Block cur = info.CurrentContainer;
            if (cur != info.LastMatchedContainer &&
                info.Container == info.LastMatchedContainer &&
                !info.IsBlank &&
                cur.Tag == BlockTag.Paragraph &&
                cur.StringContent.Length > 0)
            {
                // create lazy continuation paragraph
                BlockParser.AddLine(cur, info.LineInfo, info.Line, info.Offset);
                return true;
            }

            return false;
        }
    }
}
