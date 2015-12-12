namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table caption element. Used in the <see cref="Block.TableCaptionData"/> property.
    /// </summary>
    public sealed class TableCaptionData
    {
        /// <summary>
        /// Gets or sets the caption placement.
        /// </summary>
        public CaptionPlacement Placement { get; set; }

        /// <summary>
        /// Gets or sets the lead-in text.
        /// </summary>
        /// <value>Lead-in text, or <c>null</c> if the element is a definition-style caption.</value>
        public string Lead { get; set; }
    }
}
