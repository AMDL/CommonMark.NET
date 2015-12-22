using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
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
    /// Horizontal rule parameters.
    /// </summary>
    public sealed class HorizontalRulerParameters
    {
        /// <summary>
        /// Gets or sets the horizontal rule character parameters.
        /// </summary>
        public HorizontalRulerDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.HorizontalRuler"/> element parser.
    /// </summary>
    public sealed class HorizontalRulerParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly HorizontalRulerParameters DefaultParameters = new HorizontalRulerParameters
        {
            Delimiters = new[]
            { 
                new HorizontalRulerDelimiterParameters('*'),
                new HorizontalRulerDelimiterParameters('-'),
                new HorizontalRulerDelimiterParameters('_'),
            },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRulerParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Horizontal rule parameters.</param>
        public HorizontalRulerParser(CommonMarkSettings settings, HorizontalRulerParameters parameters = null)
            : base(settings, BlockTag.HorizontalRuler)
        {
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
                    yield return new HorizontalRulerHandler(Settings, Tag, delimiter);
                }
            }
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

        private HorizontalRulerParameters Parameters
        {
            get;
        }
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
                // it's only now that we know the line is not part of a setext header:
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
    }
}
