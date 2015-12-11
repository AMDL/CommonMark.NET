using CommonMark.Formatters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Inline parser delegate.
    /// </summary>
    /// <param name="block">Parent block.</param>
    /// <param name="subject">Subject.</param>
    /// <returns>Inline element or <c>null</c>.</returns>
    public delegate Syntax.Inline InlineParserDelegate(Syntax.Block block, Parser.Subject subject);

    /// <summary>
    /// Extension interface.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets the mapping from character to inline parser delegate.
        /// </summary>
        IDictionary<char, InlineParserDelegate> InlineParsers { get; }

        /// <summary>
        /// Gets the mapping from character to inline tag for matched single-character openers.
        /// </summary>
        IDictionary<char, InlineTag> SingleCharTags { get; }

        /// <summary>
        /// Gets the mapping from character to inline tag for matched double-character openers.
        /// </summary>
        IDictionary<char, InlineTag> DoubleCharTags { get; }

        /// <summary>
        /// Gets the mapping from block tag to block element formatter.
        /// </summary>
        IDictionary<BlockTag, IBlockFormatter> BlockFormatters { get; }

        /// <summary>
        /// Gets the mapping from inline tag to inline element formatter.
        /// </summary>
        IDictionary<InlineTag, IInlineFormatter> InlineFormatters { get; }
    }
}
