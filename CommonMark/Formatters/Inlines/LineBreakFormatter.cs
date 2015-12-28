using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.LineBreak"/> element formatter.
    /// </summary>
    public sealed class LineBreakFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineBreakFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public LineBreakFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.LineBreak, "linebreak", "br")
        {
            IsSelfClosing = true;
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            writer.WriteConstant("<br");
            WritePosition(writer, element);
            writer.WriteLineConstant(" />");
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
            writer.WriteLine();
            return false;
        }
    }
}
