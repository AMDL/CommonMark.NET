using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.FencedCode"/> element formatter.
    /// </summary>
    public class FencedCodeFormatter : CodeBlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public FencedCodeFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.FencedCode, printerTag: "fenced_code")
        {
        }

        /// <summary>
        /// Writes the additional information that applies to the element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        protected override void WriteInfo(IHtmlTextWriter writer, Block element)
        {
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
        }

        /// <summary>
        /// Returns the additional properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Additional properties.</returns>
        protected override Dictionary<string, object> GetData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { "length", element.FencedCodeData?.FenceLength },
                { "info", printer.Format(element.FencedCodeData?.Info) },
            };
        }
    }
}
