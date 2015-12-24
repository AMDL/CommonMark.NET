using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.ThematicBreak"/> element formatter.
    /// </summary>
    public sealed class ThematicBreakFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThematicBreakFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public ThematicBreakFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.ThematicBreak, printerTag: "hrule")
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
            writer.EnsureLine();
            if (Parameters.TrackPositions)
            {
                writer.WriteConstant("<hr");
                WritePosition(writer, element);
                writer.WriteLineConstant("/>");
            }
            else
            {
                writer.WriteLineConstant("<hr />");
            }
            return false;
        }
    }
}
