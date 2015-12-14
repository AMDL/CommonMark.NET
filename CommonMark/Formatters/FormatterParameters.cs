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
            this._blockFormatters = settings.GetLazy(GetBlockFormatters);
            this._inlineFormatters = settings.GetLazy(GetInlineFormatters);
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

        private Lazy<Formatters.IBlockFormatter[]> _blockFormatters;

        /// <summary>
        /// Gets the block element formatters.
        /// </summary>
        internal Formatters.IBlockFormatter[] BlockFormatters
        {
            get { return _blockFormatters.Value; }
        }

        private Formatters.IBlockFormatter[] GetBlockFormatters()
        {
            return Settings.Extensions.GetItems(Formatters.BlockFormatter.InitializeFormatters(this),
                ext => ext.BlockFormatters, key => (int)key, DelegateBlockFormatter.Merge);
        }

        #endregion BlockFormatters

        #region InlineFormatters

        private Lazy<Formatters.IInlineFormatter[]> _inlineFormatters;

        /// <summary>
        /// Gets the inline element formatters.
        /// </summary>
        internal Formatters.IInlineFormatter[] InlineFormatters
        {
            get { return _inlineFormatters.Value; }
        }

        private Formatters.IInlineFormatter[] GetInlineFormatters()
        {
            return Settings.Extensions.GetItems(Formatters.InlineFormatter.InitializeFormatters(this),
                ext => ext.InlineFormatters, key => (int)key, DelegateInlineFormatter.Merge);
        }

        #endregion InlineFormatters

        #region Settings

        private CommonMarkSettings Settings { get; }

        #endregion Settings
    }
}
