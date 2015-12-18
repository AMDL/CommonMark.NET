namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// List item delimiter parameters.
    /// </summary>
    public struct ListItemDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemDelimiterParameters"/> structure.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="minSpaces">Minimum space count.</param>
        public ListItemDelimiterParameters(char character, int minSpaces = 1)
        {
            Character = character;
            MinSpaces = minSpaces;
        }

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of space characters between the delimiter and the item content.
        /// </summary>
        public int MinSpaces { get; set; }
    }
}
