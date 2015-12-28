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
        /// <param name="textTag">Text syntax tree tag.</param>
        protected HeadingFormatter(FormatterParameters parameters, BlockTag tag, string textTag)
            : base(parameters, tag, textTag: textTag)
        {
            IsFixedHtmlInlines = true;
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
            var headingLevel = element.Heading.Level;
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
        /// <param name="element">Block element.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(Block element)
        {
            var headingLevel = element.Heading.Level;
            return headingLevel > 6
                ? "</h" + headingLevel.ToString(CultureInfo.InvariantCulture) + ">"
                : CloserTags[headingLevel - 1];
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block element)
        {
            return new Dictionary<string, object>
            {
                { "level", element.Heading.Level },
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
            : base(parameters, BlockTag.AtxHeading, "atx_heading")
        {
        }
    }

    /// <summary>
    /// <see cref="BlockTag.SetextHeading"/> element formatter.
    /// </summary>
    public sealed class SetextHeadingFormatter : HeadingFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetextHeadingFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public SetextHeadingFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.SetextHeading, "setext_heading")
        {
        }
    }
}
