﻿using System;

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
        /// <c>#</c> will be recognized as an ordered list marker.
        /// </summary>
        OrderedSharps = 32,

        /// <summary>
        /// <c>•</c>, <c>o</c> and <c></c> will be recognized as unordered list markers.
        /// </summary>
        Bullets = 128,

        /// <summary>
        /// Unordered lists with filled circle markers.
        /// <c>●</c> will be recognized as marker.
        /// </summary>
        Discs = 256,

        /// <summary>
        /// Unordered lists with circle markers.
        /// <c>◦</c> will be recognized as marker.
        /// </summary>
        Circles = 512,

        /// <summary>
        /// Unordered lists with square markers.
        /// <c>▪</c> will be recognized as marker.
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
        Unordered = 3968,

        /// <summary>
        /// All fancy lists features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
