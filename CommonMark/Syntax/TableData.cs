namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table block elements. Used in the <see cref="Block.TableData"/> property.
    /// </summary>
    public sealed class TableData
    {
        /// <summary>
        /// Gets or sets the table type.
        /// </summary>
        public TableType TableType { get; set; }

        /// <summary>
        /// Gets or sets the first column.
        /// </summary>
        public TableColumnData FirstColumn { get; set; }

        /// <summary>
        /// Gets or sets the last column.
        /// </summary>
        public TableColumnData LastColumn { get; set; }

        /// <summary>
        /// Gets or sets the column count.
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the header column delimiter.
        /// </summary>
        public TableHeaderColumnDelimiter HeaderColumnDelimiter { get; set; }

        /// <summary>
        /// Gets or sets the column delimiter.
        /// </summary>
        public TableColumnDelimiter ColumnDelimiter { get; set; }
    }
}
