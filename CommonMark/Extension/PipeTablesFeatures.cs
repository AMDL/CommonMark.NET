namespace CommonMark.Extension
{
    /// <summary>
    /// Pipe tables features.
    /// </summary>
    [System.Flags]
    public enum PipeTablesFeatures
    {
        /// <summary>
        /// No pipe tables features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Equals will be recognized as header, footer and row group separator line markers.
        /// </summary>
        HeaderEquals = 1,

        /// <summary>
        /// Pluses in header, footer and row group separator lines will be recognized as column markers
        /// (<c>---+---</c>, similar to org table syntax).
        /// </summary>
        HeaderPlus = 2,

        /// <summary>
        /// Colons in header and footer separator lines will be recognized as alignment markers
        /// (<c>:--</c> left, <c>--:</c> right, <c>:-:</c> center, <c>---</c> default).
        /// </summary>
        HeaderColon = 4,

        /// <summary>
        /// Hyphens will be recognized as footer separator line markers.
        /// </summary>
        Footers = 8,

        /// <summary>
        /// Hyphens will be recognized as row group separator line markers.
        /// </summary>
        Groups = 16,

        /// <summary>
        /// Lines immediately preceding header lines will be recognized as column group lines.
        /// </summary>
        ColumnGroups = 32,

        /// <summary>
        /// All pipe tables features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
