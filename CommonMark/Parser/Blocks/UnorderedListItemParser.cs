using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Unordered list item parameters.
    /// </summary>
    public sealed class UnorderedListItemParameters : ListItemParameters<UnorderedListItemDelimiterParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedListItemParameters"/> class.
        /// </summary>
        /// <param name="listStyle">List style.</param>
        /// <param name="markerChars">Marker characters.</param>
        public UnorderedListItemParameters(string listStyle = null, params char[] markerChars)
            : this(delimiters: GetDelimiters(listStyle, markerChars))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedListItemParameters"/> class.
        /// </summary>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public UnorderedListItemParameters(BlockTag tag = BlockTag.ListItem, BlockTag parentTag = BlockTag.UnorderedList, ListType listType = ListType.Bullet,
            params UnorderedListItemDelimiterParameters[] delimiters)
#pragma warning restore 0618
            : base(tag, parentTag, listType, delimiters)
        {
        }

        private static UnorderedListItemDelimiterParameters[] GetDelimiters(string listStyle, char[] markerChars)
        {
            var length = markerChars.Length;
            var delimiters = new UnorderedListItemDelimiterParameters[length];
            for (var i = 0; i < length; i++)
                delimiters[i] = new UnorderedListItemDelimiterParameters(markerChars[i], listStyle: listStyle);
            return delimiters;
        }
    }
}
