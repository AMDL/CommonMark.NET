using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.FencedCode"/> element formatter.
    /// </summary>
    public class FencedCodeFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeFormatter"/> class.
        /// </summary>
        /// <param name="parameters"></param>
        public FencedCodeFormatter(FormatterParameters parameters)
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
            return element.Tag == BlockTag.FencedCode;
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

            var info = element.FencedCodeData?.Info;
            if (info != null && info.Length > 0)
            {
                var x = info.IndexOf(' ');
                if (x == -1)
                    x = info.Length;

                writer.WriteConstant(" class=\"language-");
                writer.WriteEncodedHtml(new StringPart(info, 0, x));
                writer.Write('\"');
            }

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
            return "fenced_code";
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
                { "length", element.FencedCodeData?.FenceLength },
                { "info", printer.Format(element.FencedCodeData?.Info) },
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
