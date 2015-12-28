using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.SoftBreak"/> element formatter.
    /// </summary>
    public class SoftBreakFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftBreakFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public SoftBreakFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.SoftBreak, textTag: "softbreak")
        {
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            writer.WriteLine();
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

    internal sealed class HardBreakFormatter : SoftBreakFormatter
    {
        public HardBreakFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            writer.WriteConstant("<br");
            WritePosition(writer, element);
            writer.WriteLineConstant(" />");
            return false;
        }
    }
}
