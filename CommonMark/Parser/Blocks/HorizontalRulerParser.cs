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
        /// <param name="opener">Opening character.</param>
        public HorizontalRulerParser(CommonMarkSettings settings, char opener)
            : base(settings, BlockTag.HorizontalRuler, opener)
        {
            Opener = opener;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            if (!info.IsIndented && (info.Container.Tag != BlockTag.Paragraph || info.IsAllMatched) && ScanHorizontalRule(info, Opener))
            {
                // it's only now that we know the line is not part of a setext header:
                info.Container = CreateChildBlock(info, Tag, info.FirstNonspace);
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

        private char Opener
        {
            get;
        }
    }
}
