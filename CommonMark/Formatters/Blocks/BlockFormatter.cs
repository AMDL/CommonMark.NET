using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Block element formatter.
    /// </summary>
    public abstract class BlockFormatter : IBlockFormatter
    {
        private readonly CommonMarkSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockFormatter"/> class.
        /// </summary>
        /// <param name="settings">The object containing settings for the formatting process.</param>
        protected BlockFormatter(CommonMarkSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Checks whether the formatter can handle a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns><c>true</c> if the formatter can handle the specified block element.</returns>
        public abstract bool CanHandle(Block block);

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="block">Block element.</param>
        public abstract void WriteOpening(IHtmlTextWriter writer, Block block);

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <param name="visitChildren"><c>true</c> if the parent formatter should visit the child block elements.</param>
        /// <returns>The closing.</returns>
        public abstract string GetClosing(Block block, out bool visitChildren);

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns><c>true</c>, <c>false</c> or <c>null</c>.</returns>
        public virtual bool? IsStackTight(bool tight)
        {
            return null;
        }

        /// <summary>
        /// Writes the position of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="block">Block element.</param>
        protected void WritePosition(IHtmlTextWriter writer, Block block)
        {
            if (settings.TrackSourcePosition)
                writer.WritePosition(block);
        }

        internal static IBlockFormatter[] InitializeFormatters(CommonMarkSettings settings)
        {
            var f = new IBlockFormatter[(int)BlockTag.Count];
            f[(int)BlockTag.TableCell] = new TableCellFormatter(settings);
            f[(int)BlockTag.TableRow] = new TableRowFormatter(settings);
            f[(int)BlockTag.TableCaption] = new TableCaptionFormatter(settings);
            f[(int)BlockTag.Table] = new TableFormatter(settings);
            return f;
        }
    }
}
