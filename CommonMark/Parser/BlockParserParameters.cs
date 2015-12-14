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

        private readonly Lazy<BlockAdvancerDelegate[]> _advancers;

        /// <summary>
        /// Gets the advancer delegates.
        /// </summary>
        public BlockAdvancerDelegate[] Advancers
        {
            get { return _advancers.Value; }
        }

        private BlockAdvancerDelegate[] GetAdvancers()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeAdvancers,
                ext => ext.BlockAdvancers, key => (int)key, DelegateBlockAdvancer.Merge);
        }

        #endregion Advancers

        #region Initializers

        private readonly Lazy<BlockInitializerDelegate[]> _initializers;

        /// <summary>
        /// Gets the initializer delegates.
        /// </summary>
        public BlockInitializerDelegate[] Initializers
        {
            get { return _initializers.Value; }
        }

        private BlockInitializerDelegate[] GetInitializers()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeInitializers,
                ext => ext.BlockInitializers, key => key, DelegateBlockInitializer.Merge);
        }

        #endregion Initializers

        #region Finalizers

        private readonly Lazy<BlockFinalizerDelegate[]> _finalizers;

        /// <summary>
        /// Gets the finalizer delegates.
        /// </summary>
        public BlockFinalizerDelegate[] Finalizers
        {
            get { return _finalizers.Value; }
        } 

        private BlockFinalizerDelegate[] GetFinalizers()
        {
            return Settings.Extensions.GetItems(BlockMethods.InitializeFinalizers,
                ext => ext.BlockFinalizers, key => (int)key, DelegateBlockFinalizer.Merge);
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
        /// Checks if a character can serve as a fence delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can be used for opening/closing a fence.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsFenceDelimiter(char c)
        {
            return c == '`' || c == '~';
        }

        private CommonMarkSettings Settings { get; }
    }
}
