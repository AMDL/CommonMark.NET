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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMarkExtension"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected CommonMarkExtension(CommonMarkSettings settings)
        {
            this._blockParsers = settings.GetLazy(() => InitializeBlockParsers(settings));
            this._inlineParsers = settings.GetLazy(() => InitializeInlineParsers(settings));

            this._inlineDelimiterHandlers = settings.GetLazy(InitializeInlineDelimiterHandlers);
            this._referenceNormalizer = settings.GetLazy(InitializeReferenceNormalizer);

            var parameters = settings.FormatterParameters;
            this._blockFormatters = settings.GetLazy(() => InitializeBlockFormatters(parameters));
            this._inlineFormatters = settings.GetLazy(() => InitializeInlineFormatters(parameters));
        }

        #endregion

        #region Block Parsers

        private readonly Lazy<IDictionary<BlockTag, IBlockParser>> _blockParsers;

        IDictionary<BlockTag, IBlockParser> ICommonMarkExtension.BlockParsers
        {
            get { return _blockParsers.Value; }
        }

        /// <summary>
        /// Initializes the mapping from block tag to block element parser.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected virtual IDictionary<BlockTag, IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
            return null;
        }

        /// <summary>
        /// Registers a block parser.
        /// </summary>
        /// <param name="tag">Block element tag.</param>
        /// <param name="parser">Block element parser.</param>
        /// <returns>Mapping from block tag to block element parser.</returns>
        protected IDictionary<BlockTag, IBlockParser> Register(BlockTag tag, IBlockParser parser)
        {
            return Register(_blockParsers, tag, parser);
        }

        #endregion

        #region Inline Parsers

        private readonly Lazy<ICollection<IInlineParser>> _inlineParsers;

        IEnumerable<IInlineParser> ICommonMarkExtension.InlineParsers
        {
            get { return _inlineParsers.Value; }
        }

        /// <summary>
        /// Initializes the inline parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected virtual ICollection<IInlineParser> InitializeInlineParsers(CommonMarkSettings settings)
        {
            return null;
        }

        /// <summary>
        /// Registers an inline element parser.
        /// </summary>
        /// <param name="inlineParser">Inline element parser.</param>
        /// <returns>Inline element parsers.</returns>
        protected ICollection<IInlineParser> Register(IInlineParser inlineParser)
        {
            var inlineParsers = _inlineParsers.IsValueCreated ? _inlineParsers.Value : new List<IInlineParser>();
            inlineParsers.Add(inlineParser);
            return inlineParsers;
        }

        #endregion

        #region InlineDelimiterHandlers

        private readonly Lazy<IDictionary<char, IInlineDelimiterHandler>> _inlineDelimiterHandlers;

        IDictionary<char, IInlineDelimiterHandler> ICommonMarkExtension.InlineDelimiterHandlers
        {
            get { return _inlineDelimiterHandlers.Value; }
        }

        /// <summary>
        /// Initializes the mapping from character to inline delimiter handler.
        /// </summary>
        protected virtual IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return null;
        }

        /// <summary>
        /// Registers an inline delimiter handler.
        /// </summary>
        /// <param name="c">Handled character.</param>
        /// <param name="handler">Inline delimiter handler.</param>
        /// <returns>Mapping from character to inline delimiter handler.</returns>
        protected IDictionary<char, IInlineDelimiterHandler> Register(char c, IInlineDelimiterHandler handler)
        {
            return Register(_inlineDelimiterHandlers, c, handler);
        }

        #endregion

        #region ReferenceNormalizer

        private readonly Lazy<StringNormalizerDelegate> _referenceNormalizer;

        StringNormalizerDelegate ICommonMarkExtension.ReferenceNormalizer
        {
            get { return _referenceNormalizer.Value; }
        }

        /// <summary>
        /// Initializes the reference normalizer.
        /// </summary>
        /// <returns>Reference normalizer delegate.</returns>
        protected virtual StringNormalizerDelegate InitializeReferenceNormalizer()
        {
            return null;
        }

        #endregion

        #region BlockFormatters

        private readonly Lazy<IDictionary<BlockTag, IBlockFormatter>> _blockFormatters;

        IDictionary<BlockTag, IBlockFormatter> ICommonMarkExtension.BlockFormatters
        {
            get { return _blockFormatters.Value; }
        }

        /// <summary>
        /// Initializes the mapping from block tag to block element formatter.
        /// </summary>
        protected virtual IDictionary<BlockTag, IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Registers a block element formatter.
        /// </summary>
        /// <param name="tag">Block element tag.</param>
        /// <param name="formatter">Block element formatter.</param>
        /// <returns>Mapping from block tag to block element formatter.</returns>
        protected IDictionary<BlockTag, IBlockFormatter> Register(BlockTag tag, IBlockFormatter formatter)
        {
            return Register(_blockFormatters, tag, formatter);
        }

        #endregion

        #region InlineFormatters

        private readonly Lazy<IDictionary<InlineTag, IInlineFormatter>> _inlineFormatters;

        IDictionary<InlineTag, IInlineFormatter> ICommonMarkExtension.InlineFormatters
        {
            get { return _inlineFormatters.Value; }
        }

        /// <summary>
        /// Initializes the mapping from inline tag to inline element formatter.
        /// </summary>
        protected virtual IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return null;
        }

        /// <summary>
        /// Registers an inline element formatter.
        /// </summary>
        /// <param name="tag">Inline element tag.</param>
        /// <param name="formatter">Inline element formatter.</param>
        /// <returns>Mapping from inline tag to inline element formatter.</returns>
        protected IDictionary<InlineTag, IInlineFormatter> Register(InlineTag tag, IInlineFormatter formatter)
        {
            return Register(_inlineFormatters, tag, formatter);
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
        /// <returns>Type name.</returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        #endregion

        #region Register methods

        private IDictionary<TKey, TValue> Register<TKey, TValue>(Lazy<IDictionary<TKey, TValue>> lazy, TKey key, TValue value)
        {
            var items = lazy.IsValueCreated ? lazy.Value : new Dictionary<TKey, TValue>();
            items.Add(key, value);
            return items;
        }

        #endregion
    }
}
