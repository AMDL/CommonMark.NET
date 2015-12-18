using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.ListItem"/> element formatter.
    /// </summary>
    public class ListItemFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public ListItemFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.ListItem, "li", "list_item")
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
            return tight;
        }
    }
}
