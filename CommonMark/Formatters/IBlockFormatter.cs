using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Block element formatter interface.
    /// </summary>
    public interface IBlockFormatter : IElementFormatter<Block>
    {
        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        bool WriteOpening(IHtmlTextWriter writer, Block element);

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Block element.</param>
        /// <returns>The closing.</returns>
        string GetClosing(IHtmlFormatter formatter, Block element);

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        bool? IsStackTight(Block element, bool tight);
    }
}
