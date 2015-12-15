using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Block element parser parameters.
    /// </summary>
    public class BlockParserParameters
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockParserParameters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public BlockParserParameters(CommonMarkSettings settings)
        {
            this.Settings = settings;
            this._advancers = settings.GetLazy(GetAdvancers);
            this._initializers = settings.GetLazy(GetInitializers);
            this._finalizers = settings.GetLazy(GetFinalizers);
            this._processors = settings.GetLazy(GetProcessors);
        }

        #endregion Constructor

        #region Advancers

        private readonly Lazy<BlockParserDelegate[]> _advancers;

        /// <summary>
        /// Gets the advancer delegates.
        /// </summary>
        public BlockParserDelegate[] Advancers
        {
            get { return _advancers.Value; }
        }

        private BlockParserDelegate[] GetAdvancers()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeAdvancers,
                ext => ext.BlockAdvancers, key => (int)key, DelegateBlockParser.Merge);
        }

        #endregion Advancers

        #region Initializers

        private readonly Lazy<BlockParserDelegate[]> _initializers;

        /// <summary>
        /// Gets the initializer delegates.
        /// </summary>
        public BlockParserDelegate[] Initializers
        {
            get { return _initializers.Value; }
        }

        private BlockParserDelegate[] GetInitializers()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeInitializers(Settings),
                ext => ext.BlockInitializers, key => key, DelegateBlockParser.Merge);
        }

        #endregion Initializers

        #region Finalizers

        private readonly Lazy<BlockParserDelegate[]> _finalizers;

        /// <summary>
        /// Gets the finalizer delegates.
        /// </summary>
        public BlockParserDelegate[] Finalizers
        {
            get { return _finalizers.Value; }
        } 

        private BlockParserDelegate[] GetFinalizers()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeFinalizers(Settings),
                ext => ext.BlockFinalizers, key => (int)key, DelegateBlockParser.Merge);
        }

        #endregion Finalizers

        #region Processors

        private readonly Lazy<BlockProcessorDelegate[]> _processors;

        /// <summary>
        /// Gets the processor delegates.
        /// </summary>
        public BlockProcessorDelegate[] Processors
        {
            get { return _processors.Value; }
        }

        private BlockProcessorDelegate[] GetProcessors()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeProcessors,
                ext => ext.BlockProcessors, key => (int)key, DelegateBlockProcessor.Merge);
        }

        #endregion Processors

        /// <summary>
        /// Checks if a character can serve as a fence opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can be used for opening a fence.</returns>
        public virtual bool IsFenceOpener(char c)
        {
            return IsFenceDelimiter(c);
        }

        /// <summary>
        /// Checks if a character can serve as a fence closer.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can be used for closing a fence.</returns>
        public virtual bool IsFenceCloser(char c)
        {
            return IsFenceDelimiter(c);
        }

        /// <summary>
        /// Checks if a character can serve as a fence delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can be used for both opening and closing a fence.</returns>
        protected virtual bool IsFenceDelimiter(char c)
        {
            return c == '`' || c == '~';
        }

        private CommonMarkSettings Settings { get; }
    }
}
