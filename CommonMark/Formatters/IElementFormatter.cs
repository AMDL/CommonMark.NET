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
        /// Writes the opening of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        bool WriteOpening(IHtmlTextWriter writer, T element);

        /// <summary>
        /// Returns the closing of an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>The closing.</returns>
        string GetClosing(T element);

        /// <summary>
        /// Returns the syntax tree node tag for an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        string GetPrinterTag(T element);

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        IDictionary<string, object> GetPrinterData(T element);
    }

    /// <summary>
    /// Block element formatter interface.
    /// </summary>
    public interface IBlockFormatter : IElementFormatter<Block>
    {
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
        /// Returns the content rendering option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        bool? IsRenderPlainTextInlines(Inline element);
    }
}
