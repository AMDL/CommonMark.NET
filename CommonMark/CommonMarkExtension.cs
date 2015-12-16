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

        private readonly Lazy<IDictionary<BlockTag, IBlockParser>> _blockParsers;
        private readonly Lazy<IEnumerable<IInlineParser>> _inlineParsers;
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
            this._blockParsers = settings.GetLazy(() => InitializeBlockParsers(settings));
            this._inlineParsers = settings.GetLazy(() => InitializeInlineParsers(settings));

            this._inlineDelimiterCharacters = settings.GetLazy(InitializeInlineDelimiterCharacters);
            this._referenceNormalizer = settings.GetLazy(InitializeReferenceNormalizer);

            var parameters = settings.FormatterParameters;
            this._blockFormatters = settings.GetLazy(() => InitializeBlockFormatters(parameters));
            this._inlineFormatters = settings.GetLazy(() => InitializeInlineFormatters(parameters));
        }

        #endregion

        #region Block Parsers

        /// <summary>
        /// Creates the mapping from block tag to block processor delegate.
        /// </summary>
        public IDictionary<BlockTag, IBlockParser> BlockParsers
        {
            get { return _blockParsers.Value; }
        }

        #endregion

        #region Inline Parsers

        /// <summary>
        /// Gets the inline parsers.
        /// </summary>
        public IEnumerable<IInlineParser> InlineParsers
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
        /// Creates the inline parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected virtual IEnumerable<IInlineParser> InitializeInlineParsers(CommonMarkSettings settings)
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
        /// Creates the mapping from block tag to block parser.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected virtual IDictionary<BlockTag, IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
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
    }
}
