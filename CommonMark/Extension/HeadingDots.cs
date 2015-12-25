﻿using System.Collections.Generic;
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
        /// Initializes a new instance of the <see cref="HeadingDots"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public HeadingDots(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes the block parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected override IEnumerable<IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
            var parameters = new SETextHeadingParameters()
            {
                Delimiters = new[]
                {
                    new SETextHeadingDelimiterParameters('=', 1),
                    new SETextHeadingDelimiterParameters('-', 2),
                    new SETextHeadingDelimiterParameters('.', 3, 4),
                },
            };
            yield return new SETextHeadingParser(settings, parameters);
        }
    }
}