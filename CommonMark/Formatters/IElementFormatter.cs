using System.Collections.Generic;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Base element formatter interface.
    /// </summary>
    /// <typeparam name="TElement">Type of element.</typeparam>
    /// <typeparam name="TTag">Type of element tag.</typeparam>
    public interface IElementFormatter<TElement, TTag>
    {
        /// <summary>
        /// Gets the element tag.
        /// </summary>
        /// <value>The element tag handled by this formatter.</value>
        TTag Tag { get; }

        /// <summary>
        /// Gets the text syntax tree tag.
        /// </summary>
        /// <value>Tag.</value>
        string TextTag { get; }

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        bool CanHandle(TElement element);

        /// <summary>
        /// Writes the opening of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        /// <param name="flag">Element-specific flag.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        bool WriteOpening(IHtmlTextWriter writer, TElement element, bool flag);

        /// <summary>
        /// Returns the closing of an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="flag">Element-specific flag.</param>
        /// <returns>The closing.</returns>
        string GetClosing(TElement element, bool flag);

        /// <summary>
        /// Determines whether inline content should be rendered as HTML.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> to render inline content as HTML.</returns>
        bool IsHtmlInlines(TElement element);

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, TElement element);
    }
}
