namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table block elements. Used in the <see cref="Block.TableData"/> property.
    /// </summary>
    public sealed class TableData
    {
        public TableColumnData[] ColumnData { get; internal set; }

        /// <summary>
        /// Gets or sets the header column delimiter.
        /// </summary>
        public TableHeaderColumnDelimiter HeaderColumnDelimiter { get; internal set; }

        /// <summary>
        /// Gets or sets the column delimiter.
        /// </summary>
        public TableColumnDelimiter ColumnDelimiter { get; internal set; }
    }
}
