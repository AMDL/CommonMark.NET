namespace CommonMark.Extension
{
    /// <summary>
    /// Fancy lists settings.
    /// </summary>
    public struct FancyListsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="features">Fancy lists features.</param>
        /// <param name="numericListStyles">Numeral list styles.</param>
        /// <param name="additiveListStyles">Additive list styles.</param>
        public FancyListsSettings(FancyListsFeatures features, NumericListStyles numericListStyles, AdditiveListStyles additiveListStyles)
        {
            this.Features = features;
            this.NumericListStyles = numericListStyles;
            this.AdditiveListStyles = additiveListStyles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="features">Fancy lists features.</param>
        public FancyListsSettings(FancyListsFeatures features)
            : this(features, NumericListStyles.None, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="numericListStyles">Numeric list styles.</param>
        public FancyListsSettings(NumericListStyles numericListStyles)
            : this(FancyListsFeatures.None, numericListStyles, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="additiveListStyles">Additive list styles.</param>
        public FancyListsSettings(AdditiveListStyles additiveListStyles)
            : this(FancyListsFeatures.None, NumericListStyles.None, additiveListStyles)
        {
        }

        /// <summary>
        /// Gets or sets the fancy lists features.
        /// </summary>
        public FancyListsFeatures Features { get; set; }

        /// <summary>
        /// Gets or sets the numeric list styles.
        /// </summary>
        public NumericListStyles NumericListStyles { get; set; }

        /// <summary>
        /// Gets or sets the additive list styles.
        /// </summary>
        public AdditiveListStyles AdditiveListStyles { get; set; }
    }
}
