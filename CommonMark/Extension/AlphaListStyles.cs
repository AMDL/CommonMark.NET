namespace CommonMark.Extension
{
    /// <summary>
    /// Extended alphabetical list styles. Used in the <see cref="FancyListsSettings.AlphaListStyles"/> property.
    /// </summary>
    public enum AlphaListStyles
    {
        /// <summary>
        /// No extended alphabetical list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Lowercase Greek letters.
        /// </summary>
        /// <remarks>
        /// If parentheses are used as delimiters, only single-letter markers will be recognized.
        /// </remarks>
        LowerGreek = 1,

        /// <summary>
        /// Lowercase Russian letters.
        /// </summary>
        /// <remarks>
        /// If parentheses are used as delimiters, only single-letter markers will be recognized.
        /// </remarks>
        LowerRussian = 2,

        /// <summary>
        /// Uppercase Russian letters.
        /// </summary>
        /// <remarks>
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// If parentheses are used as delimiters, only single-letter markers will be recognized.
        /// </remarks>
        UpperRussian = 4,

        /// <summary>
        /// Hiragana syllables.
        /// </summary>
        Hiragana = 8,

        /// <summary>
        /// Katakana syllables.
        /// </summary>
        Katakana = 16,

        /// <summary>
        /// All extended alphabetical list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
