using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Thematic break parameters.
    /// </summary>
    public sealed class ThematicBreakParameters
    {
        /// <summary>
        /// Gets or sets the thematic break character parameters.
        /// </summary>
        public ThematicBreakDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.ThematicBreak"/> element parser.
    /// </summary>
    public sealed class ThematicBreakParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly ThematicBreakParameters DefaultParameters = new ThematicBreakParameters
        {
            Delimiters = new[]
            { 
                new ThematicBreakDelimiterParameters('*'),
                new ThematicBreakDelimiterParameters('-'),
                new ThematicBreakDelimiterParameters('_'),
            },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ThematicBreakParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Thematic break parameters.</param>
        public ThematicBreakParser(CommonMarkSettings settings, ThematicBreakParameters parameters = null)
            : base(settings, BlockTag.ThematicBreak)
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
                    yield return new ThematicBreakHandler(Settings, Tag, delimiter);
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

        private ThematicBreakParameters Parameters
        {
            get;
        }
    }
}
