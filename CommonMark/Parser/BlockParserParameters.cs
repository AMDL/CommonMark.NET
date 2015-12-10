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
        }

        /// <summary>
        /// Checks if the character can serve as a fence delimiter.
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

        /// <summary>
        /// Checks if the character can serve as a table opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can open a table header line.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsTableOpener(char c)
        {
            if (0 == (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.PipeTables))
                return false;
            return c == '-' || c == '|' || c == ':';
        }
    }
}
