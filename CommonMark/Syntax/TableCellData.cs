namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table cell element. Used in the <see cref="Block.TableCell"/> property.
    /// </summary>
    public sealed class TableCellData
    {
        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        public TableColumnData ColumnData { get; set; }
    }
}
