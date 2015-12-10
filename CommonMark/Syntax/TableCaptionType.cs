namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the type of the table caption.
    /// </summary>
    [System.Flags]
    public enum TableCaptionType
    {
        /// <summary>
        /// No table captions are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// The caption paragraph immediately precedes the table.
        /// </summary>
        Before = 1,

        /// <summary>
        /// The caption paragraph immediately follows the table.
        /// </summary>
        After = 2,

        /// <summary>
        /// The caption paragraph begins with <c>:</c>.
        /// </summary>
        Lazy = 4,

        /// <summary>
        /// The caption paragraph begins with <c>Table:</c>.
        /// </summary>
        Table = 8,

        /// <summary>
        /// All table captions are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
