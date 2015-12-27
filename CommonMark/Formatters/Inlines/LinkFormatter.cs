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
            : base(parameters, InlineTag.Link, "a", textTag: "link")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline lement.</param>
        /// <param name="plaintext"><c>true</c> to render inline elements as plaintext.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element, bool plaintext, bool withinLink)
        {
            if (withinLink)
            {
                writer.Write('[');
                return true;
            }

            if (plaintext)
            {
                return true;
            }

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
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Inline lement.</param>
        /// <param name="plaintext"><c>true</c> to render inline elements as plaintext.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(IHtmlFormatter formatter, Inline element, bool plaintext, bool withinLink)
        {
            return !withinLink
                ? base.GetClosing(formatter, element, plaintext, withinLink)
                : "]";
        }

        /// <summary>
        /// Returns the link stacking option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> to stack elements within a link.</returns>
        public override bool IsStackWithinLink(Inline element, bool withinLink)
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
