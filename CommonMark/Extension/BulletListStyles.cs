namespace CommonMark.Extension
{
    /// <summary>
    /// Extended bullet list styles. Used in the <see cref="FancyListsSettings.BulletListStyles"/> property.
    /// </summary>
    public enum BulletListStyles
    {
        /// <summary>
        /// No extended bullet list styles are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Filled circle marker.
        /// </summary>
        Disc = 1,

        /// <summary>
        /// Circle marker.
        /// </summary>
        Circle = 2,

        /// <summary>
        /// Square marker.
        /// </summary>
        Square = 4,

        /// <summary>
        /// All extended bullet list styles are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
