using CommonMark.Formatters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Block parser delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="line">Line string.</param>
    /// <param name="first_nonspace">The index of the first non-space character.</param>
    /// <param name="indented"><c>true</c> if the line is indented.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="column">Column index.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockParserDelegate(Block container, string line, int first_nonspace, bool indented, ref int offset, ref int column);

    /// <summary>
    /// Inline parser delegate.
    /// </summary>
    /// <param name="parent">Parent block.</param>
    /// <param name="subject">Subject.</param>
    /// <returns>Inline element or <c>null</c>.</returns>
    public delegate Inline InlineParserDelegate(Block parent, Parser.Subject subject);

    /// <summary>
    /// Extension interface.
    /// </summary>
    public interface ICommonMarkExtension
    {
        /// <summary>
        /// Gets the mapping from character to block parser delegate.
        /// </summary>
        IDictionary<char, BlockParserDelegate> BlockParsers { get; }

        /// <summary>
        /// Gets the mapping from character to inline parser delegate.
        /// </summary>
        IDictionary<char, InlineParserDelegate> InlineParsers { get; }

        /// <summary>
        /// Gets the mapping from character to inline delimiter character parameters.
        /// </summary>
        IDictionary<char, Parser.InlineDelimiterCharacterParameters> InlineDelimiterCharacters { get; }

        /// <summary>
        /// Gets the mapping from block tag to block element formatter.
        /// </summary>
        IDictionary<Syntax.BlockTag, IBlockFormatter> BlockFormatters { get; }

        /// <summary>
        /// Gets the mapping from inline tag to inline element formatter.
        /// </summary>
        IDictionary<Syntax.InlineTag, IInlineFormatter> InlineFormatters { get; }
    }
}
