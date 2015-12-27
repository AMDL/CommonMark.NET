using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.String"/> element formatter.
    /// </summary>
    public sealed class StringFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public StringFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.String, textTag: "str")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="plaintext"><c>true</c> to render inline elements as plaintext.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element, bool plaintext, bool withinLink)
        {
            var writeTag = !plaintext && Parameters.TrackPositions;
            if (writeTag)
            {
                writer.WriteConstant("<span");
                WritePosition(writer, element);
                writer.Write('>');
            }

            writer.WriteEncodedHtml(element.LiteralContentValue);
            
            if (writeTag)
            {
                writer.WriteConstant("</span>");
            }

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
