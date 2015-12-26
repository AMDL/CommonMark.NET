using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.OrderedList"/> element parser.
    /// </summary>
    public sealed class OrderedListParser : ListParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public OrderedListParser(CommonMarkSettings settings)
            : base(settings, BlockTag.OrderedList, BlockTag.ListItem)
        {
        }

        /// <summary>
        /// Finalizes an ordered list.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            return DoFinalize(container, container.OrderedListData);
        }
    }
}
