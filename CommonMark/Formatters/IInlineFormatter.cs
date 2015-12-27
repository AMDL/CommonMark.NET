﻿using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Inline element formatter interface.
    /// </summary>
    public interface IInlineFormatter : IElementFormatter<Inline, InlineTag>
    {
        /// <summary>
        /// Writes the plain text opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's rendering option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        bool WritePlaintextOpening(IHtmlTextWriter writer, Inline element, bool withinLink);

        /// <summary>
        /// Returns the infix of an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns>The infix.</returns>
        string GetInfix(Inline element);

        /// <summary>
        /// Returns the plain text closing of an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's rendering option.</param>
        /// <returns>The closing.</returns>
        string GetPlaintextClosing(Inline element, bool withinLink);

        /// <summary>
        /// Determines whether inline content should be rendered as plain text.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> to render the child inlines as plain text.</returns>
        bool IsPlaintextInlines(Inline element);

        /// <summary>
        /// Determines whether inline content is to be rendered within a link.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's rendering option.</param>
        /// <returns><c>true</c> to render elements within a link.</returns>
        bool IsWithinLink(Inline element, bool withinLink);
    }
}
