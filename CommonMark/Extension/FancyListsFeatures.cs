using System;

namespace CommonMark.Extension
{
    /// <summary>
    /// Fancy lists features. Used in the <see cref="FancyListsSettings.Features"/> property.
    /// </summary>
    [Flags]
    public enum FancyListsFeatures
    {
        /// <summary>
        /// No fancy lists features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Ordered lists with lowercase ASCII letter markers.
        /// </summary>
        LowerLatin = 1,

        /// <summary>
        /// Ordered lists with uppercase ASCII letter markers.
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// </summary>
        UpperLatin = 2,

        /// <summary>
        /// Ordered lists with lowercase Roman numeral markers.
        /// </summary>
        LowerRoman = 4,

        /// <summary>
        /// Ordered lists with uppercase Roman numeral markers.
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// </summary>
        UpperRoman = 8,

        /// <summary>
        /// Unordered lists with filled circle markers.
        /// <c>●</c> will be recognized as the marker.
        /// </summary>
        Disc = 256,

        /// <summary>
        /// Unordered lists with circle markers.
        /// <c>○</c> will be recognized as the marker.
        /// </summary>
        Circle = 512,

        /// <summary>
        /// Unordered lists with square markers.
        /// <c>■</c> will be recognized as the marker.
        /// </summary>
        Square = 1024,

        /// <summary>
        /// Unordered lists with no markers.
        /// <c>∙</c> will be recognized as the marker.
        /// </summary>
        Unbulleted = 2048,

        /// <summary>
        /// All fancy lists features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
