using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Setext header delimiter parameters.
    /// </summary>
    public sealed class SETextHeaderDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderDelimiterParameters"/> structure.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="headerLevel">Header level.</param>
        /// <param name="minCount">Minimum character count.</param>
        public SETextHeaderDelimiterParameters(char character, int headerLevel, int minCount = 1)
        {
            Character = character;
            HeaderLevel = headerLevel;
            MinCount = minCount;
        }

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the header level.
        /// </summary>
        public int HeaderLevel { get; set; }

        /// <summary>
        /// Gets or sets the minimum character count.
        /// </summary>
        public int MinCount { get; set; }
    }

    /// <summary>
    /// Setext header parameters.
    /// </summary>
    public sealed class SETextHeaderParameters
    {
        /// <summary>
        /// Gets or sets the delimiter parameters.
        /// </summary>
        public SETextHeaderDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.SETextHeader"/> element parser.
    /// </summary>
    public sealed class SETextHeaderParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly SETextHeaderParameters DefaultParameters = new SETextHeaderParameters
        {
            Delimiters = new[]
            {
                new SETextHeaderDelimiterParameters('=', 1),
                new SETextHeaderDelimiterParameters('-', 2),
            },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Setext header parameters.</param>
        public SETextHeaderParser(CommonMarkSettings settings, SETextHeaderParameters parameters = null)
            : base(settings, BlockTag.SETextHeader)
        {
            // we don't count setext headers for purposes of tight/loose lists or breaking out of lists.
            IsAlwaysDiscardBlanks = true;

            Parameters = parameters ?? DefaultParameters;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                foreach (var delimiter in Parameters.Delimiters)
                {
                    yield return new SETextHeaderHandler(Settings, Tag, delimiter);
                }
            }
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            // a header can never contain more than one line
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
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

        /// <summary>
        /// Processes the inline contents of a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Process(Block container, Subject subject, ref Stack<Inline> inlineStack)
        {
            return ProcessInlines(container, subject, ref inlineStack, Settings.InlineParserParameters);
        }

        private SETextHeaderParameters Parameters
        {
            get;
        }
    }

    /// <summary>
    /// Setext header delimiter handler.
    /// </summary>
    public sealed class SETextHeaderHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="parameters">Delimiter parameters.</param>
        public SETextHeaderHandler(CommonMarkSettings settings, BlockTag tag, SETextHeaderDelimiterParameters parameters)
            : base(settings, tag, parameters.Character)
        {
            HeaderLevel = parameters.HeaderLevel;
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
                info.Container.HeaderLevel = HeaderLevel;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Scans a setext header line.
        /// Assumes that there is a <see cref="BlockDelimiterHandler.Character"/> at the current position.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>Header level, or 0 for no match.</returns>
        /// <remarks>Original: int scan_setext_header_line(string s, int pos, int sourceLength)</remarks>
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

        private int HeaderLevel
        {
            get;
        }

        private int MinCount
        {
            get;
        }
    }
}
