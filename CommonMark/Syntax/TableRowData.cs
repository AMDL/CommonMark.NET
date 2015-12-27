namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for table row element. Used in the <see cref="Block.TableRow"/> property.
    /// </summary>
    public sealed class TableRowData
    {
        /// <summary>
        /// Gets or sets the row cell count.
        /// </summary>
        public int CellCount { get; set; }
    }
}
