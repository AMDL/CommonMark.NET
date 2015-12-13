namespace CommonMark.Extension
{
    /// <summary>
    /// Definition lists features.
    /// </summary>
    [System.Flags]
    public enum DefinitionListsFeatures
    {
        /// <summary>
        /// No definition lists will be recognized. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Colons will be recognized as definition markers.
        /// </summary>
        Colon = 4, // to match TableCaptionsFeatures

        /// <summary>
        /// Tildes will be recognized as definition markers.
        /// </summary>
        Tilde = 8, // to match TableCaptionsFeatures

        /// <summary>
        /// All definition lists features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
