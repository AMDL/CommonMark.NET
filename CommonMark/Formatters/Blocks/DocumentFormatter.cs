using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.Document"/> element formatter.
    /// </summary>
    public sealed class DocumentFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public DocumentFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.Document, textTag: "document")
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
            return true;
        }
    }
}
