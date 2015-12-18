namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for ordered list block elements. Used in <see cref="Block.OrderedListData"/> property.
    /// </summary>
    public sealed class OrderedListData
    {
        /// <summary>
        /// Gets or sets the string for the first list item.
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// Gets or sets the character that follows the number.
        /// </summary>
        public char DelimiterCharacter { get; set; }

        /// <summary>
        /// Determines whether the specified object contains matching ordered list data.
        /// </summary>
        /// <param name="obj">Candidate object.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an ordered list data object containing matching data.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as OrderedListData;
            return other != null && this.DelimiterCharacter == other.DelimiterCharacter;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return DelimiterCharacter.GetHashCode();
        }
    }
}
