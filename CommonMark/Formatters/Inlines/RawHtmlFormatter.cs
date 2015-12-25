using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.RawHtml"/> element formatter.
    /// </summary>
    public sealed class RawHtmlFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawHtmlFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public RawHtmlFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.RawHtml, printerTag: "html")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element, bool withinLink)
        {
            // cannot output source position for HTML blocks
            writer.Write(element.LiteralContentValue);
            return false;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetPrinterData(IPrinter printer, Inline element)
        {
            return new Dictionary<string, object>
            {
                { string.Empty, printer.Format(element.LiteralContent) },
            };
        }
    }
}
