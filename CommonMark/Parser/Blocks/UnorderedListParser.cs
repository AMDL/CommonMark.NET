using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.UnorderedList"/> element parser.
    /// </summary>
    public sealed class UnorderedListParser : ListParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedListParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public UnorderedListParser(CommonMarkSettings settings)
            : base(settings, BlockTag.UnorderedList, BlockTag.ListItem)
        {
        }

        /// <summary>
        /// Finalizes an unordered list.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            return DoFinalize(container, container.UnorderedList);
        }
    }
}
