using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Block element parser parameters.
    /// </summary>
    internal sealed class BlockParserParameters
    {
        private readonly CommonMarkSettings settings;
        private readonly Lazy<BlockParserDelegate[]> _parsers;

        public BlockParserParameters(CommonMarkSettings settings)
        {
            this.settings = settings;
            this._parsers = new Lazy<BlockParserDelegate[]>(GetParsers);
        }

        public BlockParserDelegate[] Parsers
        {
            get { return _parsers.Value; }
        }

        private BlockParserDelegate[] GetParsers()
        {
            return settings.GetItems(new BlockParserDelegate[127],
                ext => ext.BlockParsers, key => key, GetBlockParser);
        }

        private static BlockParserDelegate GetBlockParser(BlockParserDelegate inner, BlockParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new Parser.DelegateBlockParser(inner, outer).ParseInline
                : inner;
        }

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
    }
}
