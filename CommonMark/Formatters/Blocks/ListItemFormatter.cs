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
            : base(parameters, BlockTag.ListItem, "list_item", "li")
        {
            IsFixedOpening = true;
            IsListItem = true;
        }
    }
}
