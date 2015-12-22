using CommonMark.Syntax;
using System.Globalization;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Abstract header element formatter class.
    /// </summary>
    public abstract class HeaderFormatter : BlockFormatter
    {
        private static readonly string[] OpenerTags = { "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>" };
        private static readonly string[] CloserTags = { "</h1>", "</h2>", "</h3>", "</h4>", "</h5>", "</h6>" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        protected HeaderFormatter(FormatterParameters parameters, BlockTag tag)
            : base(parameters, tag)
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
            var headerLevel = element.HeaderLevel;
            if (Parameters.TrackPositions || headerLevel > 6)
            {
                writer.WriteConstant("<h" + headerLevel.ToString(CultureInfo.InvariantCulture));
                WritePosition(writer, element);
                writer.Write('>');
            }
            else
            {
                writer.WriteConstant(OpenerTags[headerLevel - 1]);
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
            var headerLevel = element.HeaderLevel;
            return headerLevel > 6
                ? "</h" + headerLevel.ToString(CultureInfo.InvariantCulture) + ">"
                : CloserTags[headerLevel - 1];
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
    }

    /// <summary>
    /// <see cref="BlockTag.AtxHeader"/> element formatter.
    /// </summary>
    public sealed class AtxHeaderFormatter : HeaderFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtxHeaderFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public AtxHeaderFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.AtxHeader)
        {
        }
    }

    /// <summary>
    /// <see cref="BlockTag.SETextHeader"/> element formatter.
    /// </summary>
    public sealed class SETextHeaderFormatter : HeaderFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public SETextHeaderFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.SETextHeader)
        {
        }
    }
}
