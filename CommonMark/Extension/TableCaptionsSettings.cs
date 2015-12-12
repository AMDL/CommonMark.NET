namespace CommonMark.Extension
{
    /// <summary>
    /// Table captions settings.
    /// </summary>
    public struct TableCaptionsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCaptionsSettings"/> structure.
        /// </summary>
        /// <param name="features">Table captions features.</param>
        /// <param name="leads">Lead-in strings to match.</param>
        public TableCaptionsSettings(TableCaptionsFeatures features, params string[] leads)
        {
            this.Features = features;
            this.Leads = leads;
        }

        /// <summary>
        /// Gets or sets the table captions features.
        /// </summary>
        public TableCaptionsFeatures Features { get; set; }

        /// <summary>
        /// Gets or sets the strings that will be recognized as caption lead-ins.
        /// If <c>null</c>, only definition-style table captions will be recognized.
        /// </summary>
        public string[] Leads { get; set; }
    }
}
