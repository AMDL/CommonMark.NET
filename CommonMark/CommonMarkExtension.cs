using CommonMark.Formatters;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Base extension class.
    /// </summary>
    public abstract class CommonMarkExtension : ICommonMarkExtension
    {
        private readonly Lazy<IDictionary<BlockTag, IBlockFormatter>> _blockFormatters;
        private readonly Lazy<IDictionary<InlineTag, IInlineFormatter>> _inlineFormatters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMarkExtension"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected CommonMarkExtension(CommonMarkSettings settings)
        {
            var parameters = settings.FormatterParameters;
            this._blockFormatters = new Lazy<IDictionary<BlockTag, IBlockFormatter>>(() => InitializeBlockFormatters(parameters));
            this._inlineFormatters = new Lazy<IDictionary<InlineTag, IInlineFormatter>>(() => InitializeInlineFormatters(parameters));
        }

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
        public IDictionary<BlockTag, IBlockFormatter> BlockFormatters
        {
            get { return _blockFormatters.Value; }
        }

        /// <summary>
        /// Gets the mapping from inline tag to inline element formatter.
        /// </summary>
        public IDictionary<InlineTag, IInlineFormatter> InlineFormatters
        {
            get { return _inlineFormatters.Value; }
        }

        /// <summary>
        /// Determines whether the specified object has the same type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if the object is an instance of the same type.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && GetType().Equals(obj.GetType());
        }

        /// <summary>
        /// Returns the hash code of the type object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        /// <summary>
        /// Returns the type name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Creates the mapping from block tag to block element formatter.
        /// </summary>
        protected virtual IDictionary<BlockTag, IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Creates the mapping from inline tag to inline element formatter.
        /// </summary>
        protected virtual IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return null;
        }
    }
}
