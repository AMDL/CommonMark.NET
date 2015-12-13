﻿using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Block element parser parameters.
    /// </summary>
    internal sealed class BlockParserParameters
    {
        #region Constructor

        public BlockParserParameters(CommonMarkSettings settings)
        {
            this.Settings = settings;
            this._parsers = new Lazy<BlockParserDelegate[]>(GetParsers);
            this._processors = new Lazy<BlockProcessorDelegate[]>(GetProcessors);
        }

        #endregion Constructor

        #region Parsers

        private readonly Lazy<BlockParserDelegate[]> _parsers;

        public BlockParserDelegate[] Parsers
        {
            get { return _parsers.Value; }
        }

        private BlockParserDelegate[] GetParsers()
        {
            return Settings.GetItems(new BlockParserDelegate[127],
                ext => ext.BlockParsers, key => key, GetParser);
        }

        private static BlockParserDelegate GetParser(BlockParserDelegate inner, BlockParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockParser(inner, outer).Parse
                : inner;
        }

        #endregion Parsers

        #region Processors

        private readonly Lazy<BlockProcessorDelegate[]> _processors;

        public BlockProcessorDelegate[] Processors
        {
            get { return _processors.Value; }
        }

        private BlockProcessorDelegate[] GetProcessors()
        {
            return Settings.GetItems(BlockMethods.InitializeProcessors(),
                ext => ext.BlockProcessors, key => (int)key, GetProcessor);
        }

        private BlockProcessorDelegate GetProcessor(BlockProcessorDelegate inner, BlockProcessorDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockProcessor(inner, outer).Process
                : inner;
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
