using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Base element formatter interface.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public interface IElementFormatter<T>
    {
        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        bool CanHandle(T element);

        /// <summary>
        /// Returns the syntax tree node tag for an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        string GetPrinterTag(T element);

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        IDictionary<string, object> GetPrinterData(IPrinter printer, T element);
    }

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

    /// <summary>
    /// Inline element formatter interface.
    /// </summary>
    public interface IInlineFormatter : IElementFormatter<Inline>
    {
        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        bool WriteOpening(IHtmlTextWriter writer, Inline element, bool withinLink);

        /// <summary>
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns>The closing.</returns>
        string GetClosing(IHtmlFormatter formatter, Inline element, bool withinLink);

        /// <summary>
        /// Returns the content rendering option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="plaintext">The parent's rendering option.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        bool? IsRenderPlainTextInlines(Inline element, bool plaintext);

        /// <summary>
        /// Returns the link stacking option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> to stack elements within a link.</returns>
        bool IsStackWithinLink(Inline element, bool withinLink);
    }
}
