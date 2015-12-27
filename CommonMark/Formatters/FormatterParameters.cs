using System;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Formatter parameters.
    /// </summary>
    public class FormatterParameters
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterParameters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public FormatterParameters(CommonMarkSettings settings)
        {
            this.Settings = settings;
            this._blockFormatters = GetBlockFormatters();
            this._inlineFormatters = GetInlineFormatters();
            this._syntaxFormatter = new SyntaxFormatter();
        }

        #endregion Constructor

        #region TrackPositions

        /// <summary>
        /// Gets or sets a value indicating whether source positions need to be written to the output.
        /// </summary>
        public bool TrackPositions
        {
            get { return Settings.TrackSourcePosition; }
        }

        #endregion TrackPositions

        #region UriResolver

        /// <summary>
        /// Gets or sets the delegate that is used to resolve addresses during rendering process.
        /// </summary>
        public Func<string, string> UriResolver
        {
            get { return Settings.UriResolver; }
        }

        #endregion UriResolver

        #region BlockFormatters

        private IBlockFormatter[] _blockFormatters;

        /// <summary>
        /// Gets the block element 
        /// </summary>
        internal IBlockFormatter[] BlockFormatters
        {
            get { return _blockFormatters; }
        }

        private IBlockFormatter[] GetBlockFormatters()
        {
            return Settings.Extensions.GetItems(
                BlockFormatter.InitializeFormatters(this), (int)Syntax.BlockTag.Count,
                ext => ext.BlockFormatters, f => (int)f.Tag, DelegateBlockFormatter.Merge,
                i => new BlockFormatter(this, (Syntax.BlockTag)i));
        }

        #endregion BlockFormatters

        #region InlineFormatters

        private IInlineFormatter[] _inlineFormatters;

        /// <summary>
        /// Gets the inline element 
        /// </summary>
        internal IInlineFormatter[] InlineFormatters
        {
            get { return _inlineFormatters; }
        }

        private IInlineFormatter[] GetInlineFormatters()
        {
            return Settings.Extensions.GetItems(
                InlineFormatter.InitializeFormatters(this), (int)Syntax.InlineTag.Count,
                ext => ext.InlineFormatters, f => (int)f.Tag, DelegateInlineFormatter.Merge,
                i => new InlineFormatter(this, (Syntax.InlineTag)i));
        }

        #endregion InlineFormatters

        #region SyntaxFormatter

        private SyntaxFormatter _syntaxFormatter;

        internal ISyntaxFormatter SyntaxFormatter
        {
            get { return _syntaxFormatter; }
        }

        #endregion SyntaxFormatter

        #region Settings

        private CommonMarkSettings Settings { get; }

        #endregion Settings
    }
}
