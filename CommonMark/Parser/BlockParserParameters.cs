using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Block element parser parameters.
    /// </summary>
    class BlockParserParameters
    {
        private readonly CommonMarkSettings settings;

        public BlockParserParameters(CommonMarkSettings settings)
        {
            this.settings = settings;
            this._setextHeaderDelimiters = new Lazy<char[]>(GetSETextHeaderDelimiters);
        }

        /// <summary>
        /// Checks if the character is a fence delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can be used for opening/closing a fence.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsFenceDelimiter(char c)
        {
            return c == '`' || c == '~' ||
                (c == ':' && 0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.CustomContainers));
        }

        /// <summary>
        /// Checks if the character is a setext header delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can be used in a setext header line.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsSETextHeaderDelimiter(char c)
        {
            return GetSETextHeaderLevel(c, 0) > 0;
        }

        /// <summary>
        /// Returns the level of a setext header.
        /// </summary>
        /// <param name="c">Header delimiter.</param>
        /// <param name="length">Row length.</param>
        /// <returns>Header level or 0.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal int GetSETextHeaderLevel(char c, int length)
        {
            if (c == '.' && length == 1)
                return 0;
            var delims = SetextHeaderDelimiters;
            return Array.IndexOf(delims, c, 0, delims.Length) + 1;
        }

        private readonly Lazy<char[]> _setextHeaderDelimiters;

        private char[] SetextHeaderDelimiters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _setextHeaderDelimiters.Value; }
        } 

        private char[] GetSETextHeaderDelimiters()
        {
            var handleDot = 0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.HeaderDots);
            var delims = handleDot ? new char[3] : new char[2];
            delims[0] = '=';
            delims[1] = '-';
            if (handleDot)
                delims[2] = '.';
            return delims;
        }
    }
}
