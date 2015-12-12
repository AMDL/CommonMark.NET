namespace CommonMark.Extension
{
    /// <summary>
    /// Definition lists settings.
    /// </summary>
    public struct DefinitionListsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionListsSettings"/> structure.
        /// </summary>
        /// <param name="features">Defintion lists features.</param>
        public DefinitionListsSettings(DefinitionListsFeatures features)
        {
            this.Features = features;
        }

        /// <summary>
        /// Gets or sets the definition lists features.
        /// </summary>
        public DefinitionListsFeatures Features { get; set; }
    }
}
