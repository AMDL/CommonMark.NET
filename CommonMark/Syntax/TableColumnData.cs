namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains table column data.
    /// </summary>
    public sealed class TableColumnData
    {
        /// <summary>
        /// Gets or sets the column alignment.
        /// </summary>
        public TableColumnAlignment Alignment { get; internal set; }

        /// <summary>
        /// Gets or sets the next column. Returns <c>null</c> if this is the last column.
        /// </summary>
        public TableColumnData NextSibling { get; internal set; }
    }
}
