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
            IsFixedOpening = true;
        }
    }
}
