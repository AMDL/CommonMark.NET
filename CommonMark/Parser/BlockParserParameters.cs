using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    /// <summary>
    /// Stage 1 block initializer delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockInitializerDelegate(ref BlockParserInfo info);

    /// <summary>
    /// Stage 1 block opener delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockOpenerDelegate(ref BlockParserInfo info);

    /// <summary>
    /// Stage 1 block closer delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockCloserDelegate(BlockParserInfo info);

    /// <summary>
    /// Stage 1 block finalizer delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockFinalizerDelegate(Block container);

    /// <summary>
    /// Stage 2 block processor delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="inlineStack">Inline stack.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockProcessorDelegate(Block container, Subject subject, ref Stack<Inline> inlineStack);

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
            this._parsers = Settings.GetLazy(GetParsers);
            this._initializers = Settings.GetLazy(GetInitializers);
            this._openers = Settings.GetLazy(GetOpeners);
            this._closers = Settings.GetLazy(GetClosers);
            this._finalizers = Settings.GetLazy(GetFinalizers);
            this._processors = Settings.GetLazy(GetProcessors);
        }

        #endregion Constructor

        #region Parsers

        private readonly Lazy<IBlockParser[]> _parsers;

        /// <summary>
        /// Gets the array mapping from block tag to block element parser.
        /// </summary>
        /// <value>Mapping from block element tag to parser.</value>
        public IBlockParser[] Parsers
        {
            get { return _parsers.Value; }
        }

        private IBlockParser[] GetParsers()
        {
            return Settings.Extensions.GetItems(BlockParser.InitializeParsers(Settings),
                ext => ext.BlockParsers, key => (int)key, DelegateBlockParser.Merge);
        }

        #endregion Parsers

        #region Initializers

        private readonly Lazy<BlockInitializerDelegate[]> _initializers;

        /// <summary>
        /// Gets the initializer delegates.
        /// </summary>
        /// <value>Mapping from block element to initializer delegate.</value>
        internal BlockInitializerDelegate[] Initializers
        {
            get { return _initializers.Value; }
        }

        private BlockInitializerDelegate[] GetInitializers()
        {
            return GetItems<BlockInitializerDelegate>(p => p.Initialize);
        }

        #endregion Initializers

        #region Openers

        private readonly Lazy<BlockOpenerDelegate[]> _openers;

        /// <summary>
        /// Gets the opener delegates.
        /// </summary>
        /// <value>Mapping from character tag to opener delegate.</value>
        internal BlockOpenerDelegate[] Openers
        {
            get { return _openers.Value; }
        }

        private BlockOpenerDelegate[] GetOpeners()
        {
            var i = new Dictionary<char, BlockOpenerDelegate>();
            var max = (char)0;
            BlockOpenerDelegate opener;
            char[] chars;
            foreach (var parser in Parsers)
            {
                if (parser != null && (chars = parser.Characters) != null)
                {
                    foreach (var c in chars)
                    {
                        if (c != (char)0)
                        {
                            i.TryGetValue(c, out opener);
                            i[c] = DelegateBlockOpener.Merge(opener, parser.Open);
                            if (c > max)
                                max = c;
                        }
                    }
                }
            }

            var openers = new BlockOpenerDelegate[max + 1];
            foreach (var kvp in i)
            {
                openers[kvp.Key] = kvp.Value;
            }

            return openers;
        }

        #endregion Openers

        #region Closers

        private readonly Lazy<BlockCloserDelegate[]> _closers;

        /// <summary>
        /// Gets the closer delegates.
        /// </summary>
        /// <value>Mapping from block element tag to closer delegate.</value>
        internal BlockCloserDelegate[] Closers
        {
            get { return _closers.Value; }
        }

        private BlockCloserDelegate[] GetClosers()
        {
            return GetItems<BlockCloserDelegate>(p => p.Close);
        }

        #endregion Closers

        #region Finalizers

        private readonly Lazy<BlockFinalizerDelegate[]> _finalizers;

        /// <summary>
        /// Gets the finalizer delegates.
        /// </summary>
        /// <value>Mapping from block element tag to finalizer delegate.</value>
        internal BlockFinalizerDelegate[] Finalizers
        {
            get { return _finalizers.Value; }
        }

        private BlockFinalizerDelegate[] GetFinalizers()
        {
            return GetItems<BlockFinalizerDelegate>(p => p.Finalize);
        }

        #endregion Finalizers

        #region Processors

        private readonly Lazy<BlockProcessorDelegate[]> _processors;

        /// <summary>
        /// Gets the processor delegates.
        /// </summary>
        /// <value>Mapping from block element tag to processor delegate.</value>
        internal BlockProcessorDelegate[] Processors
        {
            get { return _processors.Value; }
        }

        private BlockProcessorDelegate[] GetProcessors()
        {
            return GetItems<BlockProcessorDelegate>(p => p.Process);
        }

        #endregion Processors

        /// <summary>
        /// Returns the parser delegates of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of delegate.</typeparam>
        /// <param name="itemFactory">Item factory.</param>
        /// <returns>Mapping from block element tag to <typeparamref name="T"/>.</returns>
        protected T[] GetItems<T>(Func<IBlockParser, T> itemFactory)
        {
            var parsers = Parsers;
            var items = new T[(int)BlockTag.Count];
            for (int i = 0; i < (int)BlockTag.Count; i++)
            {
                if (parsers[i] != null)
                {
                    items[i] = itemFactory(parsers[i]);
                }
            }
            return items;
        }

        private CommonMarkSettings Settings { get; }
    }
}
