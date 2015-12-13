using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Base code block formatter class.
    /// </summary>
    public abstract class CodeBlockFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockFormatter"/> class.
        /// </summary>
        /// <param name="parameters"></param>
        protected CodeBlockFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Writes the opening of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            writer.EnsureLine();
            writer.WriteConstant("<pre><code");
            WritePosition(writer, element);
            WriteInfo(writer, element);
            writer.Write('>');
            writer.WriteEncodedHtml(element.StringContent);
            writer.WriteLineConstant("</code></pre>");
            return false;
        }

        /// <summary>
        /// Returns the closing of an element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(IHtmlFormatter formatter, Block element)
        {
            return null;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block element)
        {
            var data = GetData(printer, element);
            data.Add(string.Empty, printer.Format(element.StringContent));
            return data;
        }

        /// <summary>
        /// Gets the HTML tag for the element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        protected override string GetTag(Block element)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Writes the additional information that applies to the element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        protected virtual void WriteInfo(IHtmlTextWriter writer, Block element)
        {
        }

        /// <summary>
        /// Returns the additional properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Additional Properties.</returns>
        protected virtual Dictionary<string, object> GetData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>();
        }
    }
}
