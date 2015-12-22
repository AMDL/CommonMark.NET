using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list item parameters.
    /// </summary>
    public sealed class BulletListItemParameters : ListItemParameters<BulletListItemDelimiterParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemParameters"/> class.
        /// </summary>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public BulletListItemParameters(BlockTag tag = BlockTag.ListItem, BlockTag parentTag = BlockTag.BulletList, ListType listType = ListType.Bullet,
            params BulletListItemDelimiterParameters[] delimiters)
#pragma warning restore 0618
            : base(tag, parentTag, listType, delimiters)
        {
        }
    }
}
