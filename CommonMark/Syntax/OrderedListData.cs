namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for ordered list block elements. Used in <see cref="Block.OrderedListData"/> property.
    /// </summary>
    public sealed class OrderedListData : ListData<OrderedListData>
    {
        /// <summary>
        /// Gets or sets the ordinal value of the first list item.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the character that follows the number.
        /// </summary>
        public char DelimiterCharacter { get; set; }

        /// <summary>
        /// Gets or sets the list marker type.
        /// </summary>
        public OrderedListMarkerType MarkerType { get; set; }

        /// <summary>
        /// Determines whether the specified object contains matching ordered list data.
        /// </summary>
        /// <param name="obj">Candidate object.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an ordered list data object containing matching data.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as OrderedListData;
            return other != null
                && this.DelimiterCharacter == other.DelimiterCharacter
                && this.MarkerType == other.MarkerType
                && base.Equals(obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode()
                ^ DelimiterCharacter.GetHashCode()
                ^ MarkerType.GetHashCode();
        }
    }
}
