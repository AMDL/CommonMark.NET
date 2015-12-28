using CommonMark.Formatters;
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
        /// Initializes the block parsing properties.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        void InitializeBlockParsing(CommonMarkSettings settings);

        /// <summary>
        /// Initializes the inline parsing properties.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        void InitializeInlineParsing(CommonMarkSettings settings);

        /// <summary>
        /// Initializes the formatting properties.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        void InitializeFormatting(FormatterParameters parameters);

        /// <summary>
        /// Gets the block parsers.
        /// </summary>
        IEnumerable<IBlockParser> BlockParsers { get; }

        /// <summary>
        /// Gets the block delimiter handlers.
        /// </summary>
        IEnumerable<IBlockDelimiterHandler> BlockDelimiterHandlers { get; }

        /// <summary>
        /// Gets the inline parsers.
        /// </summary>
        IEnumerable<IInlineParser> InlineParsers { get; }

        /// <summary>
        /// Gets the inline delimiter handlers.
        /// </summary>
        IEnumerable<IInlineDelimiterHandler> InlineDelimiterHandlers { get; }

        /// <summary>
        /// Gets the escapable characters.
        /// </summary>
        IEnumerable<char> EscapableCharacters { get; }

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
