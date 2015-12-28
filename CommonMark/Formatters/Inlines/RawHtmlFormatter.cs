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
            : base(parameters, InlineTag.RawHtml, textTag: "html")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            // cannot output source position for HTML blocks
            writer.Write(element.LiteralContentValue);
            return false;
        }

        /// <summary>
        /// Writes the plaintext opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WritePlaintextOpening(IHtmlTextWriter writer, Inline element)
        {
            // cannot output source position for HTML blocks
            writer.Write(element.LiteralContentValue);
            return false;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Inline element)
        {
            return new Dictionary<string, object>
            {
                { string.Empty, formatter.Format(element.LiteralContent) },
            };
        }
    }
}
