using CommonMark.Formatters;
using CommonMark.Parser;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Stage 1 block parser delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="line">Line string.</param>
    /// <param name="startIndex">The index of the first non-space character.</param>
    /// <param name="indented"><c>true</c> if the line is indented.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="column">Column index.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockParserDelegate(Block container, string line, int startIndex, bool indented, ref int offset, ref int column);

    /// <summary>
    /// Stage 2 block processor delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="referenceMap">The reference mapping used when parsing links.</param>
    /// <param name="settings">Common settings.</param>
    /// <param name="inlineStack">Inline stack.</param>
    /// <returns><c>true</c> if successful.</returns>
    public delegate bool BlockProcessorDelegate(Block container, Subject subject, Dictionary<string, Reference> referenceMap, CommonMarkSettings settings, ref Stack<Inline> inlineStack);

    /// <summary>
    /// Inline parser delegate.
    /// </summary>
    /// <param name="parent">Parent block.</param>
    /// <param name="subject">Subject.</param>
    /// <returns>Inline element or <c>null</c>.</returns>
    public delegate Inline InlineParserDelegate(Block parent, Subject subject);

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
        /// Gets the mapping from block tag to block processor delegate.
        /// </summary>
        IDictionary<Syntax.BlockTag, BlockProcessorDelegate> BlockProcessors { get; }

        /// <summary>
        /// Gets the mapping from character to inline parser delegate.
        /// </summary>
        IDictionary<char, InlineParserDelegate> InlineParsers { get; }

        /// <summary>
        /// Gets the mapping from character to inline delimiter character parameters.
        /// </summary>
        IDictionary<char, InlineDelimiterCharacterParameters> InlineDelimiterCharacters { get; }

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
