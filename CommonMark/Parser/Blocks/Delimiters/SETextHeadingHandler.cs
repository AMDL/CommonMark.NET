using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Setext heading delimiter parameters.
    /// </summary>
    public sealed class SETextHeadingDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeadingDelimiterParameters"/> structure.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="headingLevel">Heading level.</param>
        /// <param name="minCount">Minimum character count.</param>
        public SETextHeadingDelimiterParameters(char character, int headingLevel, int minCount = 1)
        {
            Character = character;
            HeadingLevel = headingLevel;
            MinCount = minCount;
        }

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the heading level.
        /// </summary>
        public int HeadingLevel { get; set; }

        /// <summary>
        /// Gets or sets the minimum character count.
        /// </summary>
        public int MinCount { get; set; }
    }

    /// <summary>
    /// Setext heading delimiter handler.
    /// </summary>
    public sealed class SETextHeadingHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeadingHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="parameters">Delimiter parameters.</param>
        public SETextHeadingHandler(CommonMarkSettings settings, BlockTag tag, SETextHeadingDelimiterParameters parameters)
            : base(settings, tag, parameters.Character)
        {
            Character = parameters.Character;
            HeadingLevel = parameters.HeadingLevel;
            MinCount = parameters.MinCount;
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            if (!info.IsIndented && info.Container.Tag == BlockTag.Paragraph && ScanLine(info)
                && BlockParser.ContainsSingleLine(info.Container.StringContent))
            {
                info.Container.Tag = Tag;
                info.Container.HeaderLevel = HeadingLevel;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Matches a setext heading line.
        /// Assumes that there is a <see cref="Character"/> at the current position.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>Heading level, or 0 for no match.</returns>
        /// <remarks>Original: int scan_setext_heading_line(string s, int pos, int sourceLength)</remarks>
        private bool ScanLine(BlockParserInfo info)
        {
            var line = info.Line;
            var offset = info.FirstNonspace;
            var length = line.Length;

            // [=]+ [ ]* [\n] { return 1; }
            // [-]+ [ ]* [\n] { return 2; }
            // .? { return 0; }

            if (offset >= length - MinCount)
                return false;

            var fin = false;
            while (++offset < length)
            {
                var curChar = line[offset];

                if (curChar == Character && !fin)
                    continue;

                if (offset - info.FirstNonspace < MinCount)
                    return false;

                fin = true;

                if (curChar == ' ')
                    continue;

                if (curChar == '\n')
                    break;

                return false;
            }

            return true;
        }

        private char Character
        {
            get;
        }

        private int HeadingLevel
        {
            get;
        }

        private int MinCount
        {
            get;
        }
    }
}
