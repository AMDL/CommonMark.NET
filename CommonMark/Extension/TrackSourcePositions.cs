using CommonMark.Formatters;
using CommonMark.Parser;

namespace CommonMark.Extension
{
    /// <summary>
    /// Track precise positions in the source data for block and inline elements.
    /// </summary>
    public sealed class TrackSourcePositions : CommonMarkExtension
    {
        /// <summary>
        /// Initializes the block parsing properties.
        /// </summary>
        /// <param name="parameters">Block parser paramters.</param>
        public override void InitializeBlockParsing(BlockParserParameters parameters)
        {
            parameters.TrackPositions = true;
        }

        /// <summary>
        /// Initializes the formatting properties.
        /// </summary>
        /// <param name="parameters">Formatter paramters.</param>
        public override void InitializeFormatting(FormatterParameters parameters)
        {
            parameters.TrackPositions = true;
        }
    }
}
