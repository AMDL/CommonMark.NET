namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table caption element. Used in the <see cref="Block.TableCaptionData"/> property.
    /// </summary>
    public sealed class TableCaptionData
    {
        /// <summary>
        /// Gets or sets the caption type.
        /// </summary>
        public TableCaptionType CaptionType { get; internal set; }
    }
}
