using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.HtmlBlock"/> element formatter.
    /// </summary>
    public sealed class HtmlBlockFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlockFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public HtmlBlockFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.HtmlBlock, printerTag: "html_block")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            // cannot output source position for HTML blocks
            writer.Write(element.StringContent);
            return false;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { string.Empty, printer.Format(element.StringContent) },
            };
        }
    }
}
