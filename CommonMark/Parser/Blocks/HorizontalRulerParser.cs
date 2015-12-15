using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.HorizontalRuler"/> element parser.
    /// </summary>
    public class HorizontalRulerParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRulerParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public HorizontalRulerParser(CommonMarkSettings settings)
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
                return new[] { '*', '-', '_' };
            }
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            return false;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            if (!info.IsIndented && !(info.Container.Tag == BlockTag.Paragraph && !info.IsAllMatched) && 0 != (Scanner.scan_hrule(info.Line, info.FirstNonspace, info.Line.Length)))
            {
                // it's only now that we know the line is not part of a setext header:
                info.Container = CreateChildBlock(info, BlockTag.HorizontalRuler, info.FirstNonspace);
                BlockMethods.Finalize(info.Container, info.LineInfo, Settings);
                info.Container = info.Container.Parent;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
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
            return true;
        }
    }
}
