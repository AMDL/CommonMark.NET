using CommonMark.Parser.Blocks.Delimiters;
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
}
