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
        /// <remarks>
        /// If parentheses are used as delimiters, markers cannot be longer than than three letters.
        /// </remarks>
        LowerRoman = 1,

        /// <summary>
        /// Ordered lists with uppercase Roman numeral markers.
        /// </summary>
        /// <remarks>
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// If parentheses are used as delimiters, markers cannot be longer than than three letters.
        /// </remarks>
        UpperRoman = 2,

        /// <summary>
        /// All Roman numeral lists are enabled.
        /// </summary>
        Roman = 3,

        /// <summary>
        /// Ordered lists with lowercase ASCII letter markers.
        /// </summary>
        /// <remarks>
        /// If parentheses are used as delimiters, only single-letter markers will be recognized.
        /// </remarks>
        LowerLatin = 4,

        /// <summary>
        /// Ordered lists with uppercase ASCII letter markers.
        /// </summary>
        /// <remarks>
        /// If dots are used as delimiters, at least two spaces after the dot are required.
        /// If parentheses are used as delimiters, only single-letter markers will be recognized.
        /// </remarks>
        UpperLatin = 8,

        /// <summary>
        /// All ASCII letter lists are enabled.
        /// </summary>
        Latin = 12,

        /// <summary>
        /// <c>#</c> will be recognized as an ordered list marker.
        /// </summary>
        OrderedHashes = 32,

        /// <summary>
        /// Unordered lists with filled circle markers.
        /// <c>•</c> will be recognized as marker.
        /// </summary>
        Discs = 256,

        /// <summary>
        /// Unordered lists with circle markers.
        /// <c>o</c> will be recognized as marker.
        /// </summary>
        Circles = 512,

        /// <summary>
        /// Unordered lists with square markers.
        /// <c></c> will be recognized as marker.
        /// </summary>
        Squares = 1024,

        /// <summary>
        /// Unordered lists with no markers.
        /// <c>∙</c> will be recognized as marker.
        /// </summary>
        Unbulleted = 2048,

        /// <summary>
        /// All extended unordered lists are enabled.
        /// </summary>
        Unordered = 3840,

        /// <summary>
        /// List types will be rendered in the output.
        /// </summary>
        OutputTypes = 4096,

        /// <summary>
        /// List styles will be rendered in the output.
        /// </summary>
        OutputStyles = 8192,

        /// <summary>
        /// All fancy lists features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
