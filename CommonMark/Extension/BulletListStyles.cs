namespace CommonMark.Extension
{
    /// <summary>
    /// Extended unordered list styles. Used in the <see cref="FancyListsSettings.BulletListStyles"/> property.
    /// </summary>
    public enum BulletListStyles
    {
        /// <summary>
        /// No extended unordered list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Unordered list with filled circle markers.
        /// </summary>
        Disc = 1,

        /// <summary>
        /// Unordered list with circle markers.
        /// </summary>
        Circle = 2,

        /// <summary>
        /// Unordered list with square markers.
        /// </summary>
        Square = 4,

        /// <summary>
        /// Unordered list with no markers.
        /// </summary>
        Unbulleted = 8,

        /// <summary>
        /// All extended unordered list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
