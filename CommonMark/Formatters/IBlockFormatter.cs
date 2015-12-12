using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters
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
        /// <returns><c>true</c> if the formatter can handle <paramref name="block"/>.</returns>
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
        /// <param name="block">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        bool? IsStackTight(Block block, bool tight);

        /// <summary>
        /// Returns the syntax tree node tag for a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns>Tag.</returns>
        string GetPrinterTag(Block block);

        /// <summary>
        /// Returns the properties of a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        IDictionary<string, object> GetPrinterData(Block block);
    }
}
