using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Horizontal rule character parameters.
    /// </summary>
    public sealed class HorizontalRulerDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRulerDelimiterParameters"/> class.
        /// </summary>
        /// <param name="character">Horizontal rule character.</param>
        /// <param name="minCount">Minimum character count.</param>
        public HorizontalRulerDelimiterParameters(char character, int minCount = 3)
        {
            this.Character = character;
            this.MinCount = minCount;
        }

        /// <summary>
        /// Gets or sets the horizontal rule character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the minimum character count.
        /// </summary>
        public int MinCount { get; set; }
    }

    /// <summary>
    /// Horizontal rule delimiter handler.
    /// </summary>
    public sealed class HorizontalRulerHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRulerHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="parameters">Horizontal rule character parameters.</param>
        public HorizontalRulerHandler(CommonMarkSettings settings, BlockTag tag, HorizontalRulerDelimiterParameters parameters)
            : base(settings, tag, parameters.Character)
        {
            Character = parameters.Character;
            MinCount = parameters.MinCount;
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            if (!info.IsIndented && (info.Container.Tag != BlockTag.Paragraph || info.IsAllMatched) && ScanHorizontalRule(info, Character, MinCount))
            {
                // it's only now that we know the line is not part of a setext heading:
                info.Container = AppendChildBlock(info, Tag, info.FirstNonspace);
                BlockMethods.Finalize(info.Container, info.LineInfo, Settings);
                info.Container = info.Container.Parent;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }

        private int MinCount
        {
            get;
        }

        private char Character
        {
            get;
        }
    }
}
