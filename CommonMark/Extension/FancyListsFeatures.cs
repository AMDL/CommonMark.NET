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
        /// Ordered lists with lowercase Roman numeral markers.
        /// </summary>
        LowerRoman = 1,

        /// <summary>
        /// Ordered lists with uppercase Roman numeral markers.
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// </summary>
        UpperRoman = 2,

        /// <summary>
        /// All Roman numeral lists are enabled.
        /// </summary>
        Roman = 3,

        /// <summary>
        /// Ordered lists with lowercase ASCII letter markers.
        /// </summary>
        LowerLatin = 4,

        /// <summary>
        /// Ordered lists with uppercase ASCII letter markers.
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// </summary>
        UpperLatin = 8,

        /// <summary>
        /// All ASCII letter lists are enabled.
        /// </summary>
        Latin = 12,

        /// <summary>
        /// A sharp followed by a dot will be recognized as a decimal list marker.
        /// </summary>
        DecimalSharps = 32,

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
        /// All extended unordered lists are enabled.
        /// </summary>
        Unordered = 3840,

        /// <summary>
        /// All fancy lists features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
