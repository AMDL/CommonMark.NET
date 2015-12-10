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

        /// <summary>
        /// Checks if a character can serve as a pipe table header line opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can open a pipe table header line.</returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsPipeTableOpener(char c)
        {
            return (0 == (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.PipeTables))
                && (IsPipeTableHeaderDelimiter(c) || IsPipeTableColumnDelimiter(c) || IsPipeTableHeaderAlignmentMarker(c));
        }

        /// <summary>
        /// Checks if a character can serve as a pipe table header line delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a pipe table header line delimiter.
        /// </returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsPipeTableHeaderDelimiter(char c)
        {
            return c == '-';
        }

        /// <summary>
        /// Checks if a character can serve as a column delimiter in a pipe table header line.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a column delimiter in a pipe table header line.
        /// </returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsPipeTableHeaderColumnDelimiter(char c)
        {
            return IsPipeTableColumnDelimiter(c) || (c == '+' && 0 != (settings.PipeTables & PipeTableFeatures.HeaderPlus));
        }

        /// <summary>
        /// Checks if a character can serve both as a column delimiter and as a header line opener in a pipe table.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a pipe table column delimiter and can open a pipe table header line.
        /// </returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsPipeTableColumnDelimiter(char c)
        {
            return c == '|';
        }

        /// <summary>
        /// Checks if a character can serve as a pipe table header alignment marker.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as an alignment marker in a pipe table header line.
        /// </returns>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsPipeTableHeaderAlignmentMarker(char c)
        {
            return (c == ':' && 0 != (settings.PipeTables & PipeTableFeatures.HeaderColon));
        }
    }
}
