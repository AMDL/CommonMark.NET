namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table cell element. Used in the <see cref="Block.TableCellData"/> property.
    /// </summary>
    public sealed class TableCellData
    {
        /// <summary>
        /// Gets or sets the cell type.
        /// </summary>
        public TableCellType CellType { get; set; }

        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        public TableColumnData ColumnData { get; set; }
    }
}
