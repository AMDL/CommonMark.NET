using CommonMark.Formatters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Extension.
    /// </summary>
    public abstract class CommonMarkExtension : ICommonMarkExtension
    {
        /// <summary>
        /// Gets the mapping from character to inline parser delegate.
        /// </summary>
        public virtual IDictionary<char, InlineParserDelegate> InlineParsers
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the mapping from character to inline tag for matched single-character openers.
        /// </summary>
        public virtual IDictionary<char, InlineTag> SingleCharTags
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the mapping from character to inline tag for matched double-character openers.
        /// </summary>
        public virtual IDictionary<char, InlineTag> DoubleCharTags
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the mapping from block tag to block element formatter.
        /// </summary>
        public virtual IDictionary<BlockTag, IBlockFormatter> BlockFormatters
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the mapping from inline tag to inline element formatter.
        /// </summary>
        public virtual IDictionary<InlineTag, IInlineFormatter> InlineFormatters
        {
            get { return null; }
        }
    }
}
