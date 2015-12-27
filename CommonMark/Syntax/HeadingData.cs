namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for heading elements. Used in the <see cref="Block.Heading"/> property.
    /// </summary>
    public struct HeadingData
    {
        /// <summary>
        /// Gets or sets the heading level.
        /// </summary>
        public byte Level { get; set; }
    }
}
