using System;

namespace CommonMark.Extension
{
    /// <summary>
    /// Extended numeric list styles. Used in the <see cref="FancyListsSettings.NumericListStyles"/> property.
    /// </summary>
    [Flags]
    public enum NumericListStyles
    {
        /// <summary>
        /// No extended numeric list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Arabic-Indic numerals.
        /// </summary>
        ArabicIndic = 1,

        /// <summary>
        /// Extended Arabic numerals.
        /// </summary>
        Persian = 2,

        /// <summary>
        /// Thai numerals.
        /// </summary>
        Thai = 4,

        /// <summary>
        /// Cambodian numerals.
        /// </summary>
        Cambodian = 8,

        /// <summary>
        /// Khmer numerals.
        /// </summary>
        Khmer = 16,

        /// <summary>
        /// Lao numerals.
        /// </summary>
        Lao = 32,

        /// <summary>
        /// Myanmar numerals.
        /// </summary>
        Myanmar = 64,

        /// <summary>
        /// Shan numerals.
        /// </summary>
        Shan = 128,

        /// <summary>
        /// Devanagari numerals.
        /// </summary>
        Devanagari = 256,

        /// <summary>
        /// Bengali numerals.
        /// </summary>
        Bengali = 512,

        /// <summary>
        /// Eastern Nagari numerals.
        /// </summary>
        EasternNagari = 1024,

        /// <summary>
        /// Gujarati numerals.
        /// </summary>
        Gujarati = 2048,

        /// <summary>
        /// Gurmukhi numerals.
        /// </summary>
        Gurmukhi = 4096,

        /// <summary>
        /// Kannada numerals.
        /// </summary>
        Kannada = 8192,

        /// <summary>
        /// Lepcha numerals.
        /// </summary>
        Lepcha = 16384,

        /// <summary>
        /// Malayalam numerals.
        /// </summary>
        Malayalam = 32768,

        /// <summary>
        /// Marathi numerals.
        /// </summary>
        Marathi = 65536,

        /// <summary>
        /// Oriya numerals.
        /// </summary>
        Oriya = 131072,

        /// <summary>
        /// Tamil numerals.
        /// </summary>
        Tamil = 262144,

        /// <summary>
        /// Telugu numerals.
        /// </summary>
        Telugu = 524288,

        /// <summary>
        /// Tibetan numerals.
        /// </summary>
        Tibetan = 1048576,

        /// <summary>
        /// Mongolian numerals.
        /// </summary>
        Mongolian = 2097152,

        /// <summary>
        /// CJK decimal numerals.
        /// </summary>
        CJKDecimal = 4194304,

        /// <summary>
        /// Full-width decimal numerals.
        /// </summary>
        FullWidthDecimal = 8388608,

        /// <summary>
        /// All extended numeric list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
