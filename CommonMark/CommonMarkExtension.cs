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
        #region Fields

        private readonly Lazy<IDictionary<BlockTag, BlockAdvancerDelegate>> _blockAdvancers;
        private readonly Lazy<IDictionary<char, BlockInitializerDelegate>> _blockInitializers;
        private readonly Lazy<IDictionary<BlockTag, BlockFinalizerDelegate>> _blockFinalizers;
        private readonly Lazy<IDictionary<BlockTag, BlockProcessorDelegate>> _blockProcessors;
        private readonly Lazy<IDictionary<char, InlineParserDelegate>> _inlineParsers;
        private readonly Lazy<IDictionary<char, InlineDelimiterCharacterParameters>> _inlineDelimiterCharacters;
        private readonly Lazy<StringNormalizerDelegate> _referenceNormalizer;
        private readonly Lazy<IDictionary<BlockTag, IBlockFormatter>> _blockFormatters;
        private readonly Lazy<IDictionary<InlineTag, IInlineFormatter>> _inlineFormatters;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMarkExtension"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected CommonMarkExtension(CommonMarkSettings settings)
        {
            this._blockAdvancers = new Lazy<IDictionary<BlockTag, BlockAdvancerDelegate>>(InitializeBlockAdvancers);
            this._blockInitializers = new Lazy<IDictionary<char, BlockInitializerDelegate>>(InitializeBlockInitializers);
            this._blockFinalizers = new Lazy<IDictionary<BlockTag, BlockFinalizerDelegate>>(InitializeBlockFinalizers);
            this._blockProcessors = new Lazy<IDictionary<BlockTag, BlockProcessorDelegate>>(InitializeBlockProcessors);

            this._inlineParsers = new Lazy<IDictionary<char, InlineParserDelegate>>(InitalizeInlineParsers);
            this._inlineDelimiterCharacters = new Lazy<IDictionary<char, InlineDelimiterCharacterParameters>>(InitializeInlineDelimiterCharacters);
            this._referenceNormalizer = new Lazy<StringNormalizerDelegate>(InitializeReferenceNormalizer);

            var parameters = settings.FormatterParameters;
            this._blockFormatters = new Lazy<IDictionary<BlockTag, IBlockFormatter>>(() => InitializeBlockFormatters(parameters));
            this._inlineFormatters = new Lazy<IDictionary<InlineTag, IInlineFormatter>>(() => InitializeInlineFormatters(parameters));
        }

        #endregion

        #region Block Parsers

        /// <summary>
        /// Gets the mapping from block tag to block advancer delegate.
        /// </summary>
        public IDictionary<BlockTag, BlockAdvancerDelegate> BlockAdvancers
        {
            get { return _blockAdvancers.Value; }
        }

        /// <summary>
        /// Gets the mapping from character to block initializer delegate.
        /// </summary>
        public IDictionary<char, BlockInitializerDelegate> BlockInitializers
        {
            get { return _blockInitializers.Value; }
        }

        /// <summary>
        /// Gets the mapping from block tag to block finalizer delegate.
        /// </summary>
        public IDictionary<BlockTag, BlockFinalizerDelegate> BlockFinalizers
        {
            get { return _blockFinalizers.Value; }
        }

        /// <summary>
        /// Creates the mapping from block tag to block processor delegate.
        /// </summary>
        public IDictionary<BlockTag, BlockProcessorDelegate> BlockProcessors
        {
            get { return _blockProcessors.Value; }
        }

        #endregion

        #region Inline Parsers

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
        /// Gets the reference normalizer.
        /// </summary>
        public StringNormalizerDelegate ReferenceNormalizer
        {
            get { return _referenceNormalizer.Value; }
        }

        #endregion

        #region Formatters

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

        #endregion

        #region Object overrides

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

        #endregion

        #region Initialize methods

        /// <summary>
        /// Creates the mapping from block tag to block advancer delegate.
        /// </summary>
        protected virtual IDictionary<BlockTag, BlockAdvancerDelegate> InitializeBlockAdvancers()
        {
            return null;
        }

        /// <summary>
        /// Creates the mapping from character to block initializer delegate.
        /// </summary>
        protected virtual IDictionary<char, BlockInitializerDelegate> InitializeBlockInitializers()
        {
            return null;
        }

        /// <summary>
        /// Creates the mapping from block tag to block finalizer delegate.
        /// </summary>
        protected virtual IDictionary<BlockTag, BlockFinalizerDelegate> InitializeBlockFinalizers()
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

        /// <summary>
        /// Creates the reference normalizer.
        /// </summary>
        /// <returns>Reference normalizer delegate.</returns>
        protected virtual StringNormalizerDelegate InitializeReferenceNormalizer()
        {
            return null;
        }

        #endregion

        #region Helper methods
		
        /// <summary>
        /// Processes the inline contents of a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="refmap">The reference mapping used when parsing links.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <returns></returns>
        protected bool ProcessInlines(Block block, Subject subject, Dictionary<string, Reference> refmap, ref Stack<Inline> inlineStack, InlineParserParameters parameters)
        {
            return BlockMethods.ProcessInlines(block, subject, refmap, ref inlineStack, parameters);
        }

	    #endregion
    }
}
