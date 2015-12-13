using CommonMark.Formatters;
using CommonMark.Parser;
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
        private readonly Lazy<IDictionary<char, BlockParserDelegate>> _blockParsers;
        private readonly Lazy<IDictionary<BlockTag, BlockProcessorDelegate>> _blockProcessors;
        private readonly Lazy<IDictionary<char, InlineParserDelegate>> _inlineParsers;
        private readonly Lazy<IDictionary<char, InlineDelimiterCharacterParameters>> _inlineDelimiterCharacters;
        private readonly Lazy<IDictionary<BlockTag, IBlockFormatter>> _blockFormatters;
        private readonly Lazy<IDictionary<InlineTag, IInlineFormatter>> _inlineFormatters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMarkExtension"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected CommonMarkExtension(CommonMarkSettings settings)
        {
            var parameters = settings.FormatterParameters;
            this._blockParsers = new Lazy<IDictionary<char, BlockParserDelegate>>(InitalizeBlockParsers);
            this._blockProcessors = new Lazy<IDictionary<BlockTag, BlockProcessorDelegate>>(InitializeBlockProcessors);
            this._inlineParsers = new Lazy<IDictionary<char, InlineParserDelegate>>(InitalizeInlineParsers);
            this._inlineDelimiterCharacters = new Lazy<IDictionary<char, InlineDelimiterCharacterParameters>>(InitializeInlineDelimiterCharacters);
            this._blockFormatters = new Lazy<IDictionary<BlockTag, IBlockFormatter>>(() => InitializeBlockFormatters(parameters));
            this._inlineFormatters = new Lazy<IDictionary<InlineTag, IInlineFormatter>>(() => InitializeInlineFormatters(parameters));
        }

        /// <summary>
        /// Gets the mapping from character to block parser delegate.
        /// </summary>
        public IDictionary<char, BlockParserDelegate> BlockParsers
        {
            get { return _blockParsers.Value; }
        }

        /// <summary>
        /// Creates the mapping from block tag to block processor delegate.
        /// </summary>
        public IDictionary<BlockTag, BlockProcessorDelegate> BlockProcessors
        {
            get { return _blockProcessors.Value; }
        }

        /// <summary>
        /// Gets the mapping from character to inline parser delegate.
        /// </summary>
        public IDictionary<char, InlineParserDelegate> InlineParsers
        {
            get { return _inlineParsers.Value; }
        }

        /// <summary>
        /// Gets the mapping from character to inline delimiter character parameters.
        /// </summary>
        public IDictionary<char, InlineDelimiterCharacterParameters> InlineDelimiterCharacters
        {
            get { return _inlineDelimiterCharacters.Value; }
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
        /// Creates the mapping from character to block parser delegate.
        /// </summary>
        protected virtual IDictionary<char, BlockParserDelegate> InitalizeBlockParsers()
        {
            return null;
        }

        /// <summary>
        /// Creates the mapping from character to inline parser delegate.
        /// </summary>
        protected virtual IDictionary<char, InlineParserDelegate> InitalizeInlineParsers()
        {
            return null;
        }

        /// <summary>
        /// Creates the mapping from character to inline delimiter character parameters.
        /// </summary>
        protected virtual IDictionary<char, InlineDelimiterCharacterParameters> InitializeInlineDelimiterCharacters()
        {
            return null;
        }

        /// <summary>
        /// Creates the mapping from block tag to block processor delegate.
        /// </summary>
        public virtual IDictionary<BlockTag, BlockProcessorDelegate> InitializeBlockProcessors()
        {
            return null;
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
