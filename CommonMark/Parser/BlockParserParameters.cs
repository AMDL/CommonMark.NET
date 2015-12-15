using CommonMark.Syntax;
using System;
using System.Collections.Generic;

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
            this._parsers = Settings.GetLazy(GetParsers);
            this._advancers = Settings.GetLazy(GetAdvancers);
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

        #region Advancers

        private readonly Lazy<BlockAdvancerDelegate[]> _advancers;

        /// <summary>
        /// Gets the advancer delegates.
        /// </summary>
        /// <value>Mapping from block element to advancer delegate.</value>
        internal BlockAdvancerDelegate[] Advancers
        {
            get { return _advancers.Value; }
        }

        private BlockAdvancerDelegate[] GetAdvancers()
        {
            return GetItems<BlockAdvancerDelegate>(p => p.Advance);
        }

        #endregion Advancers

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
                        i.TryGetValue(c, out opener);
                        i[c] = DelegateBlockOpener.Merge(opener, parser.Open);
                        if (c > max)
                            max = c;
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
