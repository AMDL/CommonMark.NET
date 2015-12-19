using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Horizontal rule parameters.
    /// </summary>
    public sealed class HorizontalRulerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRulerParameters"/> class.
        /// </summary>
        /// <param name="characters">Horizontal rule characters.</param>
        public HorizontalRulerParameters(params char[] characters)
        {
            this.Characters = characters;
        }

        /// <summary>
        /// Gets or sets the horizontal rule characters.
        /// </summary>
        public char[] Characters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.HorizontalRuler"/> element parser.
    /// </summary>
    public sealed class HorizontalRulerParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly HorizontalRulerParameters DefaultParameters = new HorizontalRulerParameters('*', '-', '_');

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
                foreach (var character in Parameters.Characters)
                {
                    yield return new HorizontalRulerHandler(Settings, character);
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
    /// Horizontal rule element handler.
    /// </summary>
    public sealed class HorizontalRulerHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRulerHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        public HorizontalRulerHandler(CommonMarkSettings settings, char character)
            : base(settings, character)
        {
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            if (!info.IsIndented && (info.Container.Tag != BlockTag.Paragraph || info.IsAllMatched) && ScanHorizontalRule(info, Character))
            {
                // it's only now that we know the line is not part of a setext header:
                info.Container = BlockParser.CreateChildBlock(info, BlockTag.HorizontalRuler, info.FirstNonspace, Settings);
                BlockMethods.Finalize(info.Container, info.LineInfo, Settings);
                info.Container = info.Container.Parent;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }
    }
}
