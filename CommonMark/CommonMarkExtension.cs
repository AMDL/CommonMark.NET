using CommonMark.Formatters;
using CommonMark.Parser;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Base extension class.
    /// </summary>
    public abstract class CommonMarkExtension : ICommonMarkExtension
    {
        #region InitializeBlockParsing

        /// <summary>
        /// Initializes the block parsing properties.
        /// </summary>
        /// <param name="parameters">Block parser parameters.</param>
        public virtual void InitializeBlockParsing(BlockParserParameters parameters)
        {
            _blockParsers = InitializeBlockParsers(parameters.Settings);
            _blockDelimiterHandlers = InitializeBlockDelimiterHandlers(parameters.Settings);
        }

        #endregion

        #region Block Parsers

        private IEnumerable<IBlockParser> _blockParsers;

        IEnumerable<IBlockParser> ICommonMarkExtension.BlockParsers
        {
            get { return _blockParsers; }
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

        private IEnumerable<IBlockDelimiterHandler> _blockDelimiterHandlers;

        IEnumerable<IBlockDelimiterHandler> ICommonMarkExtension.BlockDelimiterHandlers
        {
            get
            {
                return _blockDelimiterHandlers;
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

        #region InitializeInlineParsing

        /// <summary>
        /// Initializes the inline parsing properties.
        /// </summary>
        /// <param name="parameters">Inline parser parameters.</param>
        public virtual void InitializeInlineParsing(InlineParserParameters parameters)
        {
            _inlineParsers = InitializeInlineParsers(parameters.Settings);
            _inlineDelimiterHandlers = InitializeInlineDelimiterHandlers();
            _escapableCharacters = InitializeEscapableCharacters();
        }

        #endregion

        #region Inline Parsers

        private IEnumerable<IInlineParser> _inlineParsers;

        IEnumerable<IInlineParser> ICommonMarkExtension.InlineParsers
        {
            get { return _inlineParsers; }
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

        private IEnumerable<IInlineDelimiterHandler> _inlineDelimiterHandlers;

        IEnumerable<IInlineDelimiterHandler> ICommonMarkExtension.InlineDelimiterHandlers
        {
            get { return _inlineDelimiterHandlers; }
        }

        /// <summary>
        /// Initializes the inline delimiter handlers.
        /// </summary>
        protected virtual IEnumerable<IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return null;
        }

        #endregion

        #region Escapable Characters

        private IEnumerable<char> _escapableCharacters;

        IEnumerable<char> ICommonMarkExtension.EscapableCharacters
        {
            get { return _escapableCharacters; }
        }

        /// <summary>
        /// Initializes the escapable characters.
        /// </summary>
        protected virtual IEnumerable<char> InitializeEscapableCharacters()
        {
            return null;
        }

        #endregion

        #region InitializeFormatting

        /// <summary>
        /// Initializes the formatting properties.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public virtual void InitializeFormatting(FormatterParameters parameters)
        {
            _blockFormatters = InitializeBlockFormatters(parameters);
            _inlineFormatters = InitializeInlineFormatters(parameters);
        }

        #endregion

        #region BlockFormatters

        private IEnumerable<IBlockFormatter> _blockFormatters;

        IEnumerable<IBlockFormatter> ICommonMarkExtension.BlockFormatters
        {
            get { return _blockFormatters; }
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

        private IEnumerable<IInlineFormatter> _inlineFormatters;

        IEnumerable<IInlineFormatter> ICommonMarkExtension.InlineFormatters
        {
            get { return _inlineFormatters; }
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
    }
}
