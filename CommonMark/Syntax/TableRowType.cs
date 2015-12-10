namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the type of the table row.
    /// </summary>
    /// <remarks>Note that <see cref="TableRowType.First"/> and <see cref="TableRowType.Last"/> may be set simultaneously.</remarks>
    [System.Flags]
    public enum TableRowType
    {
        /// <summary>
        /// Regular table body row.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// First table body row.
        /// </summary>
        First = 1,

        /// <summary>
        /// Last table body row.
        /// </summary>
        Last = 2,

        /// <summary>
        /// Table header row.
        /// </summary>
        Header = 4,

        /// <summary>
        /// Table footer row.
        /// </summary>
        Footer = 12
    }
}
