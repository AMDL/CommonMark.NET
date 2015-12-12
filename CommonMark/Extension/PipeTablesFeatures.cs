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
        /// Equals will be recognized as header line markers.
        /// If disabled, a pipe table with equals in its header line will not be parsed as one.
        /// </summary>
        HeaderEquals = 1,

        /// <summary>
        /// Pluses in the header line will be recognized as column markers
        /// (<c>---+---</c>, similar to org table syntax).
        /// If disabled, a pipe table with pluses in its header line will not be parsed as one.
        /// </summary>
        HeaderPlus = 2,

        /// <summary>
        /// Colons in the header line will be recognized as alignment markers
        /// (<c>:--</c> left, <c>--:</c> right, <c>:-:</c> center, <c>---</c> default).
        /// If disabled, a pipe table with colons in its header line will not be parsed as one.
        /// </summary>
        HeaderColon = 4,

        /// <summary>
        /// All pipe tables features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
