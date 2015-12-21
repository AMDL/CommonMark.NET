﻿using System.Collections.Generic;

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
        /// Gets the syntax tree node tag.
        /// </summary>
        /// <value>Tag.</value>
        string PrinterTag { get; }

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        bool CanHandle(TElement element);

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        IDictionary<string, object> GetPrinterData(IPrinter printer, TElement element);
    }
}