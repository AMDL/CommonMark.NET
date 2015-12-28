using System.Collections.Generic;
using CommonMark.Parser;
using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Parser.Blocks;

namespace CommonMark.Extension
{
    /// <summary>
    /// Four or more dots will be recognized as Level 3 setext heading markers.
    /// </summary>
    public class HeadingDots : CommonMarkExtension
    {
        /// <summary>
        /// Initializes the block parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected override IEnumerable<IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
            var parameters = new SetextHeadingParameters()
            {
                Delimiters = new[]
                {
                    new SetextHeadingDelimiterParameters('=', 1),
                    new SetextHeadingDelimiterParameters('-', 2),
                    new SetextHeadingDelimiterParameters('.', 3, 4),
                },
            };
            yield return new SetextHeadingParser(settings, parameters);
        }
    }
}
