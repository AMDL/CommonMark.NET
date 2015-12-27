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
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="textTag">Text syntax tree tag.</param>
        protected CodeBlockFormatter(FormatterParameters parameters, BlockTag tag, string textTag)
            : base(parameters, tag, textTag: textTag)
        {
        }

        /// <summary>
        /// Writes the opening of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        /// <param name="tight"><c>true</c> to stack paragraphs tightly.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element, bool tight)
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
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block element)
        {
            var data = GetData(formatter, element);
            data.Add(string.Empty, formatter.Format(element.StringContent));
            return data;
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
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Additional Properties.</returns>
        protected virtual Dictionary<string, object> GetData(ISyntaxFormatter formatter, Block element)
        {
            return new Dictionary<string, object>();
        }
    }
}
