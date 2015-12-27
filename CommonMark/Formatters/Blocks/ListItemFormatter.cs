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
            : base(parameters, BlockTag.ListItem, "li", textTag: "list_item")
        {
        }

        /// <summary>
        /// Determines whether paragraph rendering should be skipped for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's rendering option.</param>
        /// <returns><c>true</c> to skip paragraph rendering.</returns>
        public override bool IsTight(Block element, bool tight)
        {
            return tight;
        }
    }
}
