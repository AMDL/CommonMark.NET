namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table block elements. Used in the <see cref="Block.Table"/> property.
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
        /// Gets or sets the header row delimiter character.
        /// </summary>
        public char HeaderDelimiter { get; set; }

        /// <summary>
        /// Gets or sets the header column delimiter character.
        /// </summary>
        public char HeaderColumnDelimiter { get; set; }

        /// <summary>
        /// Gets or sets the column delimiter character.
        /// </summary>
        public char ColumnDelimiter { get; set; }
    }
}
