﻿using System;

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
        /// </summary>
        UpperLatin = 2,

        /// <summary>
        /// Lowercase Roman numerals.
        /// </summary>
        LowerRoman = 4,

        /// <summary>
        /// Uppercase Roman numerals.
        /// </summary>
        UpperRoman = 8,

        /// <summary>
        /// All standard list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
