using System;

namespace CommonMark.Extension
{
    /// <summary>
    /// Extended alphabetical list styles. Used in the <see cref="FancyListsSettings.AdditiveListStyles"/> property.
    /// </summary>
    [Flags]
    public enum AdditiveListStyles
    {
        /// <summary>
        /// No standard alphabetical list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Hebrew letters.
        /// </summary>
        Hebrew,

        /// <summary>
        /// All standard alphabetical list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
