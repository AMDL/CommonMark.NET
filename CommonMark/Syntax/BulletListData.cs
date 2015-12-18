namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for bullet list block elements. Used in <see cref="Block.BulletListData"/> property.
    /// </summary>
    public sealed class BulletListData
    {
        /// <summary>
        /// Gets or sets the bullet character.
        /// </summary>
        public char BulletCharacter { get; set; }

        /// <summary>
        /// Determines whether the specified object contains matching bullet list data.
        /// </summary>
        /// <param name="obj">Candidate object.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a bullet list data object containing matching data.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as BulletListData;
            return other != null && this.BulletCharacter == other.BulletCharacter;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return BulletCharacter.GetHashCode();
        }
    }
}
