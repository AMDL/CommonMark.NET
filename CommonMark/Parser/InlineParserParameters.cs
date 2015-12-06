using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline parser parameters.
    /// </summary>
    internal class InlineParserParameters
    {
        private readonly Lazy<InlineParserDelegate[]> _parsers;
        private readonly Lazy<char[]> _specialCharacters;

        public InlineParserParameters(Func<InlineParserDelegate[]> parsersFactory, Func<char[]> specialCharactersFactory)
        {
            this._parsers = new Lazy<InlineParserDelegate[]>(parsersFactory);
            this._specialCharacters = new Lazy<char[]>(specialCharactersFactory);
        }

        /// <summary>
        /// Gets the delegates that parse inline elements according to these settings.
        /// </summary>
        public InlineParserDelegate[] Parsers
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _parsers.Value; }
        }

        /// <summary>
        /// Gets the characters that have special meaning for inline element parsers according to these settings.
        /// </summary>
        public char[] SpecialCharacters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _specialCharacters.Value; }
        }
    }
}
