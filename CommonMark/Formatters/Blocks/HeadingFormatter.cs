using CommonMark.Syntax;
using System.Globalization;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Base heading element formatter class.
    /// </summary>
    public abstract class HeadingFormatter : BlockFormatter
    {
        private static readonly string[] OpenerTags = { "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>" };
        private static readonly string[] CloserTags = { "</h1>", "</h2>", "</h3>", "</h4>", "</h5>", "</h6>" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadingFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="printerTag">Printer tag.</param>
        protected HeadingFormatter(FormatterParameters parameters, BlockTag tag, string printerTag)
            : base(parameters, tag, printerTag: printerTag)
        {
        }

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            writer.EnsureLine();
            var headingLevel = element.HeaderLevel;
            if (Parameters.TrackPositions || headingLevel > 6)
            {
                writer.WriteConstant("<h" + headingLevel.ToString(CultureInfo.InvariantCulture));
                WritePosition(writer, element);
                writer.Write('>');
            }
            else
            {
                writer.WriteConstant(OpenerTags[headingLevel - 1]);
            }
            return false;
        }

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Block element.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(IHtmlFormatter formatter, Block element)
        {
            var headingLevel = element.HeaderLevel;
            return headingLevel > 6
                ? "</h" + headingLevel.ToString(CultureInfo.InvariantCulture) + ">"
                : CloserTags[headingLevel - 1];
        }

        /// <summary>
        /// Returns the inline content rendering option.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="plaintext">Current inline rendering option.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        public override bool? IsRenderPlainTextInlines(Block element, bool plaintext)
        {
            return false;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { "level", element.HeaderLevel },
            };
        }
    }

    /// <summary>
    /// <see cref="BlockTag.AtxHeading"/> element formatter.
    /// </summary>
    public sealed class AtxHeadingFormatter : HeadingFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtxHeadingFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public AtxHeadingFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.AtxHeading, "atx_header")
        {
        }
    }

    /// <summary>
    /// <see cref="BlockTag.SETextHeading"/> element formatter.
    /// </summary>
    public sealed class SETextHeadingFormatter : HeadingFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeadingFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public SETextHeadingFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.SETextHeading, "setext_header")
        {
        }
    }
}
