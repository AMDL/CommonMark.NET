namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for emphasis elements. Used in the <see cref="Inline.Emphasis"/> property.
    /// </summary>
    public struct EmphasisData
    {
        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char DelimiterCharacter { get; set; }
    }
}
