using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Inline element formatter interface.
    /// </summary>
    public interface IInlineFormatter
    {
        /// <summary>
        /// Checks whether the formatter can handle an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="inline"/>.</returns>
        bool CanHandle(Inline inline);

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="inline">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child inline elements.</returns>
        bool WriteOpening(IHtmlTextWriter writer, Inline inline);

        /// <summary>
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>The closing.</returns>
        string GetClosing(Inline inline);

        /// <summary>
        /// Returns the content rendering option for an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        bool? IsRenderPlainTextInlines(Inline inline);

        /// <summary>
        /// Returns the syntax tree node tag for an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>Tag.</returns>
        string GetPrinterTag(Inline inline);

        /// <summary>
        /// Returns the properties of an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        IDictionary<string, object> GetPrinterData(Inline inline);
    }
}
