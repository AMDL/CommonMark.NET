namespace CommonMark.Extension
{
    /// <summary>
    /// Table captions features.
    /// </summary>
    [System.Flags]
    public enum TableCaptionsFeatures
    {
        /// <summary>
        /// No table captions features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Paragraphs immediately preceding tables will be recognized as captions.
        /// </summary>
        Before = 1,

        /// <summary>
        /// Paragraphs immediately following tables will be recognized as captions.
        /// </summary>
        After = 2,

        /// <summary>
        /// Paragraphs begining with <c>:</c> will be recognized as captions.
        /// </summary>
        Definition = 4,

        /// <summary>
        /// All table captions features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
