using System.Collections.Generic;
using CommonMark.Parser;
using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Parser.Blocks;

namespace CommonMark.Extension
{
    /// <summary>
    /// Four or more dots will be recognized as Level 3 setext header markers.
    /// </summary>
    public class HeaderDots : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDots"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public HeaderDots(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes the block parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected override IEnumerable<IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
            var parameters = new SETextHeaderParameters()
            {
                Delimiters = new[]
                {
                    new SETextHeaderDelimiterParameters('=', 1),
                    new SETextHeaderDelimiterParameters('-', 2),
                    new SETextHeaderDelimiterParameters('.', 3, 4),
                },
            };
            yield return new SETextHeaderParser(settings, parameters);
        }
    }
}
