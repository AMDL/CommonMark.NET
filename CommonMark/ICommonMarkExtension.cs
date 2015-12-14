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
        /// Gets the mapping from block tag to block advancer delegate.
        /// </summary>
        IDictionary<Syntax.BlockTag, BlockAdvancerDelegate> BlockAdvancers { get; }

        /// <summary>
        /// Gets the mapping from character to block initializer delegate.
        /// </summary>
        IDictionary<char, BlockInitializerDelegate> BlockInitializers { get; }

        /// <summary>
        /// Gets the mapping from block tag to block finalizer delegate.
        /// </summary>
        IDictionary<Syntax.BlockTag, BlockFinalizerDelegate> BlockFinalizers { get; }

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
        /// Gets the reference normalizer.
        /// </summary>
        StringNormalizerDelegate ReferenceNormalizer { get; }

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
