using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.String"/> element formatter.
    /// </summary>
    public sealed class StringFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public StringFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.String, printerTag: "str")
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
            if (Parameters.TrackPositions)
            {
                writer.WriteConstant("<span");
                WritePosition(writer, element);
                writer.Write('>');
            }

            writer.WriteEncodedHtml(element.LiteralContentValue);
            
            if (Parameters.TrackPositions)
            {
                writer.WriteConstant("</span>");
            }

            return false;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Inline element)
        {
            return new Dictionary<string, object>
            {
                { string.Empty, printer.Format(element.LiteralContent) },
            };
        }
    }
}
