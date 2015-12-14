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
            : base(parameters)
        {
        }

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public override bool CanHandle(Inline element)
        {
            return element.Tag == InlineTag.Link;
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
            if (withinLink)
            {
                writer.Write('[');
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
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(IHtmlFormatter formatter, Inline element, bool withinLink)
        {
            return !withinLink
                ? base.GetClosing(formatter, element)
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
        /// Returns the syntax tree node tag for an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        public override string GetPrinterTag(Inline element)
        {
            return "link";
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Inline element)
        {
            return new Dictionary<string, object>
            {
                { "url", printer.Format(element.TargetUrl) },
                { "title", printer.Format(element.LiteralContent) },
            };
        }

        /// <summary>
        /// Gets the HTML tag for the element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        protected override string GetTag(Syntax.Inline element)
        {
            return "a";
        }
    }
}
