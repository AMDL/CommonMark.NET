namespace CommonMark.Extension
{
    /// <summary>
    /// Table captions features.
    /// </summary>
    [System.Flags]
    public enum TableCaptionsFeatures
    {
        /// <summary>
        /// No table captions will be recognized. This is the default.
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
        /// Paragraphs begining with colons will be recognized as captions.
        /// </summary>
        ColonDefinition = 4,

        /// <summary>
        /// Paragraphs begining with tildes will be recognized as captions.
        /// </summary>
        TildeDefinition = 8,

        /// <summary>
        /// Spaces before lead-in delimiters will be allowed.
        /// </summary>
        TrimLead = 16,

        /// <summary>
        /// All table captions features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
