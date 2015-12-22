using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Inline element formatter interface.
    /// </summary>
    public interface IInlineFormatter : IElementFormatter<Inline, InlineTag>
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
        /// Returns the link stacking option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> to stack elements within a link.</returns>
        bool IsStackWithinLink(Inline element, bool withinLink);
    }
}
