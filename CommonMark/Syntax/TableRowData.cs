namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table row element. Used in the <see cref="Block.TableRowData"/> property.
    /// </summary>
    public class TableRowData
    {
        /// <summary>
        /// Gets or sets the row type.
        /// </summary>
        public TableRowType TableRowType { get; internal set; }
    }
}
