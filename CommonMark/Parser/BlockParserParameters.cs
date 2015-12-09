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
        /// Checks if the character is a fence delimiter.
        /// </summary>
        /// <param name="c"></param>
        /// <returns><c>true</c> if the character can be used for opening/closing a fence.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsFenceDelimiter(char c)
        {
            return c == '`' || c == '~' ||
                (c == ':' && 0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.CustomContainers));
        }
    }
}
