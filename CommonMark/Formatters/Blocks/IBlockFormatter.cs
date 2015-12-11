using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Block element formatter interface.
    /// </summary>
    public interface IBlockFormatter
    {
        /// <summary>
        /// Checks whether the formatter can handle a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns><c>true</c> if the formatter can handle the specified block element.</returns>
        bool CanHandle(Block block);

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="block">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child block elements.</returns>
        bool WriteOpening(IHtmlTextWriter writer, Block block);

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns>The closing.</returns>
        string GetClosing(Block block);

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns><c>true</c>, <c>false</c> or <c>null</c>.</returns>
        bool? IsStackTight(bool tight);
    }
}
