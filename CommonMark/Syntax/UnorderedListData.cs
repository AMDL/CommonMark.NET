namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for unordered list block elements. Used in <see cref="Block.UnorderedList"/> property.
    /// </summary>
    public sealed class UnorderedListData : ListData<UnorderedListData>
    {
        /// <summary>
        /// Gets or sets the bullet character.
        /// </summary>
        public char BulletCharacter { get; set; }

        /// <summary>
        /// Determines whether the specified object contains matching unordered list data.
        /// </summary>
        /// <param name="obj">Candidate object.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an unordered list data object containing matching data.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as UnorderedListData;
            return other != null
                && this.BulletCharacter == other.BulletCharacter
                && base.Equals(obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode()
                ^ BulletCharacter.GetHashCode();
        }
    }
}
