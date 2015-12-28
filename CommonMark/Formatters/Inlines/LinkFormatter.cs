using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.Link"/> element formatter.
    /// </summary>
    public class LinkFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public LinkFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Link, "link", "a")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline lement.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            writer.WriteConstant("<a href=\"");
            writer.WriteEncodedUrl(ResolveUri(element.TargetUrl));
            writer.Write('\"');
            if (element.LiteralContentValue.Length > 0)
            {
                writer.WriteConstant(" title=\"");
                writer.WriteEncodedHtml(element.LiteralContentValue);
                writer.Write('\"');
            }

            WritePosition(writer, element);

            writer.Write('>');
            return true;
        }

        /// <summary>
        /// Writes the plaintext opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline lement.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WritePlaintextOpening(IHtmlTextWriter writer, Inline element)
        {
            return true;
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
                { "url", formatter.Format(element.TargetUrl) },
                { "title", formatter.Format(element.LiteralContent) },
            };
        }
    }
}
