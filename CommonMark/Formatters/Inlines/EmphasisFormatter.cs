using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// Base emphasis element formatter class.
    /// </summary>
    public abstract class EmphasisFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Inline element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        /// <param name="printerTag">Printer tag. If unspecified, <paramref name="htmlTag"/> will be used.</param>
        protected EmphasisFormatter(FormatterParameters parameters, InlineTag tag, string htmlTag, string printerTag = null)
            : base(parameters, tag, htmlTag, printerTag: printerTag)
        {
        }

        /// <summary>
        /// Returns the link stacking option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's stacking option.</param>
        /// <returns><c>true</c> to stack elements within a link.</returns>
        public override bool IsStackWithinLink(Inline element, bool withinLink)
        {
            return withinLink;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetPrinterData(IPrinter printer, Inline element)
        {
            return new Dictionary<string, object>
            {
                { "delim", element.DelimiterCharacter },
            };
        }
    }
}
