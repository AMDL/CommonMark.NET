namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table cell element. Used in the <see cref="Block.TableCellData"/> property.
    /// </summary>
    public class TableCellData
    {
        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        public TableColumnData ColumnData { get; internal set; }
    }
}
