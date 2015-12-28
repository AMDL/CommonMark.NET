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
            : base(parameters, BlockTag.ThematicBreak, "thematic_break", "hr")
        {
            IsFixedOpening = true;
            IsSelfClosing = true;
        }
    }
}
