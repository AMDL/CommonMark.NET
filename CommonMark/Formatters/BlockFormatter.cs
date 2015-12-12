using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Block element formatter.
    /// </summary>
    public abstract class BlockFormatter : IBlockFormatter
    {
        private readonly FormatterParameters parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected BlockFormatter(FormatterParameters parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Checks whether the formatter can handle a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="block"/>.</returns>
        public abstract bool CanHandle(Block block);

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="block">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child block elements.</returns>
        public abstract bool WriteOpening(IHtmlTextWriter writer, Block block);

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns>The closing.</returns>
        public abstract string GetClosing(Block block);

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
        /// Returns the syntax tree node tag for a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <returns>Tag.</returns>
        public abstract string GetNodeTag(Block block);

        /// <summary>
        /// Writes the properties of a block element.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="block">Block element.</param>
        public abstract void Print(TextWriter writer, Block block);

        /// <summary>
        /// Writes the position of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="block">Block element.</param>
        protected void WritePosition(IHtmlTextWriter writer, Block block)
        {
            if (parameters.TrackPositions)
                writer.WritePosition(block);
        }

        internal static IBlockFormatter[] InitializeFormatters(FormatterParameters parameters)
        {
            var f = new IBlockFormatter[(int)BlockTag.Count];
            f[(int)BlockTag.FigureCaption] = new FigureCaptionFormatter(parameters);
            f[(int)BlockTag.Figure] = new FigureFormatter(parameters);
            return f;
        }
    }
}
