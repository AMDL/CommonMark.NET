namespace CommonMark.Extension
{
    /// <summary>
    /// Pipe tables settings.
    /// </summary>
    public struct PipeTablesSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTablesSettings"/> structure.
        /// </summary>
        /// <param name="features">Pipe tables features.</param>
        public PipeTablesSettings(PipeTablesFeatures features)
        {
            this.Features = features;
        }

        /// <summary>
        /// Gets or sets the pipe tables features.
        /// </summary>
        public PipeTablesFeatures Features { get; set; }
    }
}
