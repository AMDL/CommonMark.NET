using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Block element parser parameters.
    /// </summary>
    class BlockParserParameters
    {
        public BlockParserParameters(CommonMarkSettings settings)
        {
            this._fenceDelimiters = new Lazy<char[]>(() => GetFenceDelimiters(settings));
        }

        private readonly Lazy<char[]> _fenceDelimiters;

        /// <summary>
        /// Checks if the character is a fence delimiter.
        /// </summary>
        /// <param name="c"></param>
        /// <returns><c>true</c> if the character can be used for opening/closing a fence.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsFenceDelimiter(char c)
        {
            var fenceDelimeters = this._fenceDelimiters.Value;
            return Array.IndexOf(fenceDelimeters, c, 0, fenceDelimeters.Length) >= 0;
        }

        private char[] GetFenceDelimiters(CommonMarkSettings settings)
        {
            var fenceDelimeters = new char[2];
            fenceDelimeters[0] = '`';
            fenceDelimeters[1] = '~';
            return fenceDelimeters;
        }
    }
}
