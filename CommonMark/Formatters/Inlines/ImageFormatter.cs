using System.Collections.Generic;
using CommonMark.Syntax;
using System.Text;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.Image"/> element formatter.
    /// </summary>
    public sealed class ImageFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public ImageFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Image, textTag: "image")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline lement.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element, bool withinLink)
        {
            writer.WriteConstant("<img src=\"");
            writer.WriteEncodedUrl(ResolveUri(element.TargetUrl));
            writer.WriteConstant("\" alt=\"");
            return false;
        }

        /// <summary>
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Inline lement.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(IHtmlFormatter formatter, Inline element, bool withinLink)
        {
            var sb = new StringBuilder();
            sb.Append('\"');

            if (element.LiteralContentValue.Length > 0)
            {
                sb.Append(" title=\"");
                sb.Append(formatter.EscapeHtml(element.LiteralContentValue));
                sb.Append('\"');
            }

            if (Parameters.TrackPositions)
            {
                sb.Append(formatter.PrintPosition(element));
            }

            sb.Append(" />");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the inline content rendering option.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        public override bool? IsRenderPlainTextInlines(Inline element)
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
