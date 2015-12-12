namespace CommonMark.Extension
{
    /// <summary>
    /// Definition lists features.
    /// </summary>
    [System.Flags]
    public enum DefinitionListsFeatures
    {
        /// <summary>
        /// No definition lists features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Tildes will be recognized as definition markers.
        /// </summary>
        Tilde = 1,

        /// <summary>
        /// All definition lists features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
