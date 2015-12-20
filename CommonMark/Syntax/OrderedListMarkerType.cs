namespace CommonMark.Syntax
{
    /// <summary>
    /// Ordered list marker type.
    /// </summary>
    public enum OrderedListMarkerType
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Decimal numbers.
        /// </summary>
        Decimal = '1',

        /// <summary>
        /// Lowercase ASCII letters.
        /// </summary>
        LowerLatin = 'a',

        /// <summary>
        /// Uppercase ASCII letters.
        /// </summary>
        UpperLatin = 'A',

        /// <summary>
        /// Lowercase Roman numerals.
        /// </summary>
        LowerRoman = 'i',

        /// <summary>
        /// Uppercase Roman numerals.
        /// </summary>
        UpperRoman = 'I',
    }
}
