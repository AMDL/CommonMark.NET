namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains table column data. Used in the <see cref="TableData.ColumnData"/> property.
    /// </summary>
    public sealed class TableColumnData
    {
        /// <summary>
        /// Gets or sets the column alignment.
        /// </summary>
        public TableColumnAlignment Alignment { get; internal set; }
    }
}
