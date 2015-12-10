namespace CommonMark.Syntax
{
    /// <summary>
    /// Represents a part of <see cref="StringContent"/>.
    /// </summary>
    public struct StringPart
    {
        internal StringPart(string source, int startIndex, int length)
        {
            this.Source = source;
            this.StartIndex = startIndex;
            this.Length = length;
        }

        /// <summary>
        /// Gets or sets the string object this part is created from.
        /// </summary>
        public string Source;

        /// <summary>
        /// Gets or sets the index at which this part starts.
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// Gets or sets the length of the part.
        /// </summary>
        public int Length;

        /// <summary>
        /// Returns the content.
        /// </summary>
        /// <returns>String content.</returns>
        public override string ToString()
        {
            if (this.Source == null)
                return null;

            return this.Source.Substring(this.StartIndex, this.Length);
        }
    }
}
