using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Base list formatter class.
    /// </summary>
    public abstract class ListFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        /// <param name="printerTag">Printer tag.</param>
        public ListFormatter(FormatterParameters parameters, BlockTag tag, string htmlTag, string printerTag)
            : base(parameters, tag, htmlTag, printerTag)
        {
        }

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        public override bool? IsStackTight(Block element, bool tight)
        {
            return element.ListData.IsTight;
        }
    }
}
