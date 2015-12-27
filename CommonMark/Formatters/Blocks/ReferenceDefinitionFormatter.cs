using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.ReferenceDefinition"/> element formatter.
    /// </summary>
    public sealed class ReferenceDefinitionFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDefinitionFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public ReferenceDefinitionFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.ReferenceDefinition, textTag: "reference_def")
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
            return false;
        }
    }
}
