using CommonMark.Formatters;
using CommonMark.Parser;
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
            this._blockDelimiterHandlers = settings.GetLazy(() => InitializeBlockDelimiterHandlers(settings));
            this._inlineParsers = settings.GetLazy(() => InitializeInlineParsers(settings));
            this._inlineDelimiterHandlers = settings.GetLazy(InitializeInlineDelimiterHandlers);

            this._referenceNormalizer = settings.GetLazy(InitializeReferenceNormalizer);

            var parameters = settings.FormatterParameters;
            this._blockFormatters = settings.GetLazy(() => InitializeBlockFormatters(parameters));
            this._inlineFormatters = settings.GetLazy(() => InitializeInlineFormatters(parameters));
        }

        #endregion

        #region Block Parsers

        private readonly Lazy<IEnumerable<IBlockParser>> _blockParsers;

        IEnumerable<IBlockParser> ICommonMarkExtension.BlockParsers
        {
            get { return _blockParsers.Value; }
        }

        /// <summary>
        /// Initializes the block parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected virtual IEnumerable<IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
            return null;
        }

        #endregion

        #region Block Delimiter Handlers

        private Lazy<IEnumerable<IBlockDelimiterHandler>> _blockDelimiterHandlers;

        IEnumerable<IBlockDelimiterHandler> ICommonMarkExtension.BlockDelimiterHandlers
        {
            get
            {
                return _blockDelimiterHandlers.Value;
            }
        }

        /// <summary>
        /// Initializes the block delimiter handlers.
        /// </summary>
        protected virtual IEnumerable<IBlockDelimiterHandler> InitializeBlockDelimiterHandlers(CommonMarkSettings settings)
        {
            return null;
        }

        #endregion

        #region Inline Parsers

        private readonly Lazy<IEnumerable<IInlineParser>> _inlineParsers;

        IEnumerable<IInlineParser> ICommonMarkExtension.InlineParsers
        {
            get { return _inlineParsers.Value; }
        }

        /// <summary>
        /// Initializes the inline parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected virtual IEnumerable<IInlineParser> InitializeInlineParsers(CommonMarkSettings settings)
        {
            return null;
        }

        #endregion

        #region Inline Delimiter Handlers

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

        private readonly Lazy<IEnumerable<IBlockFormatter>> _blockFormatters;

        IEnumerable<IBlockFormatter> ICommonMarkExtension.BlockFormatters
        {
            get { return _blockFormatters.Value; }
        }

        /// <summary>
        /// Initializes the block formatters.
        /// </summary>
        protected virtual IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            return null;
        }

        #endregion

        #region InlineFormatters

        private readonly Lazy<IEnumerable<IInlineFormatter>> _inlineFormatters;

        IEnumerable<IInlineFormatter> ICommonMarkExtension.InlineFormatters
        {
            get { return _inlineFormatters.Value; }
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected virtual IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return null;
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
