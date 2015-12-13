namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for a caption element. Used in the <see cref="Block.CaptionData"/> property.
    /// </summary>
    public sealed class CaptionData
    {
        /// <summary>
        /// Gets or sets the caption placement.
        /// </summary>
        public CaptionPlacement Placement { get; set; }

        /// <summary>
        /// Gets or sets the lead-in text.
        /// </summary>
        public string Lead { get; set; }
    }
}
