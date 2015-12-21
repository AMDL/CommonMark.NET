using System;

namespace CommonMark.Extension
{
    /// <summary>
    /// Standard list styles. Used in the <see cref="FancyListsSettings.StandardListStyles"/> property.
    /// </summary>
    [Flags]
    public enum StandardListStyles
    {
        /// <summary>
        /// No standard list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Lowercase ASCII letters.
        /// </summary>
        LowerLatin = 1,

        /// <summary>
        /// Uppercase ASCII letters.
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// </summary>
        UpperLatin = 2,

        /// <summary>
        /// Lowercase Roman numerals.
        /// </summary>
        LowerRoman = 4,

        /// <summary>
        /// Uppercase Roman numerals.
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// </summary>
        UpperRoman = 8,

        /// <summary>
        /// All standard list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
