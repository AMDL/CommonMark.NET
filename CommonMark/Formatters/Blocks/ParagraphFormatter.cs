using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.Paragraph"/> element formatter.
    /// </summary>
    public sealed class ParagraphFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public ParagraphFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.Paragraph, textTag: "paragraph")
        {
        }

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <param name="tight"><c>true</c> to stack paragraphs tightly.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element, bool tight)
        {
            if (!tight)
            {
                writer.WriteConstant("<p");
                WritePosition(writer, element);
                writer.Write('>');
            }
            return true;
        }

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight"><c>true</c> to stack paragraphs tightly.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(Block element, bool tight)
        {
            return !tight ? "</p>" : null;
        }

        /// <summary>
        /// Determines whether inline content should be rendered as HTML.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> to render the child inlines as HTML.</returns>
        public override bool IsHtmlInlines(Block element)
        {
            return true;
        }
    }
}
