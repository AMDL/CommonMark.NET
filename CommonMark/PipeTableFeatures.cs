namespace CommonMark
{
    /// <summary>
    /// Lists features that can be enabled in <see cref="TableSettings.PipeTables"/>.
    /// These are only applicable if <see cref="CommonMarkAdditionalFeatures.PipeTables"/> is enabled.
    /// </summary>
    [System.Flags]
    public enum PipeTableFeatures
    {
        /// <summary>
        /// No pipe table features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Pluses in the header line will be recognized as column markers
        /// (<c>---+---</c>, similar to org table syntax).
        /// If disabled, a pipe table with pluses in its header line will not be parsed as one.
        /// </summary>
        HeaderPlus = 1,

        /// <summary>
        /// Colons in the header line will be recognized as alignment markers
        /// (<c>:--</c> left, <c>--:</c> right, <c>:-:</c> center, <c>---</c> default).
        /// If disabled, a pipe table with colons in its header line will not be parsed as one.
        /// </summary>
        HeaderColon = 2,

        /// <summary>
        /// All pipe table features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
