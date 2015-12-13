using CommonMark.Syntax;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.IndentedCode"/> element formatter.
    /// </summary>
    public class IndentedCodeFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedCodeFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public IndentedCodeFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public override bool CanHandle(Block element)
        {
            return element.Tag == BlockTag.IndentedCode;
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
        /// Returns the syntax tree node tag for an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        public override string GetPrinterTag(Block element)
        {
            return "indented_code";
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { string.Empty, printer.Format(element.StringContent) },
            };
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
    }
}
