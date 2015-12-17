﻿using CommonMark.Formatters;
using CommonMark.Parser;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Extension interface.
    /// </summary>
    public interface ICommonMarkExtension
    {
        /// <summary>
        /// Gets the mapping from block tag to block parser.
        /// </summary>
        IDictionary<Syntax.BlockTag, IBlockParser> BlockParsers { get; }

        /// <summary>
        /// Gets the inline parsers.
        /// </summary>
        IEnumerable<IInlineParser> InlineParsers { get; }

        /// <summary>
        /// Gets the mapping from character to inline delimiter handler.
        /// </summary>
        IDictionary<char, IInlineDelimiterHandler> InlineDelimiterHandlers { get; }

        /// <summary>
        /// Gets the reference normalizer.
        /// </summary>
        StringNormalizerDelegate ReferenceNormalizer { get; }

        /// <summary>
        /// Gets the block formatters.
        /// </summary>
        IEnumerable<IBlockFormatter> BlockFormatters { get; }

        /// <summary>
        /// Gets the inline formatters.
        /// </summary>
        IEnumerable<IInlineFormatter> InlineFormatters { get; }
    }
}
