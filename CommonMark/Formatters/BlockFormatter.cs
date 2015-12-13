using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Block element formatter.
    /// </summary>
    public abstract class BlockFormatter : ElementFormatter<Block>, IBlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected BlockFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        public virtual bool? IsStackTight(Block block, bool tight)
        {
            return null;
        }

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            writer.EnsureLine();
            return base.WriteOpening(writer, element);
        }

        /// <summary>
        /// Writes the position of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        protected override void DoWritePosition(IHtmlTextWriter writer, Block element)
        {
            writer.WritePosition(element);
        }

        internal static IBlockFormatter[] InitializeFormatters(FormatterParameters parameters)
        {
            var f = new IBlockFormatter[(int)BlockTag.Count];
            return f;
        }
    }
}
