﻿using System;

namespace CommonMark.Extension
{
    /// <summary>
    /// Extended additive list styles. Used in the <see cref="FancyListsSettings.AdditiveListStyles"/> property.
    /// </summary>
    [Flags]
    public enum AdditiveListStyles
    {
        /// <summary>
        /// No extended additive list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Hebrew letters.
        /// </summary>
        /// <remarks>
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// If parentheses are used as delimiters, only single letters will be recognized as markers.
        /// </remarks>
        Hebrew,

        /// <summary>
        /// All extended additive list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
