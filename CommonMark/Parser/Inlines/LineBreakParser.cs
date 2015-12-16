using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Line break parser.
    /// </summary>
    public class LineBreakParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineBreakParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public LineBreakParser(CommonMarkSettings settings)
            : base(settings, '\n')
        {
        }

        /// <summary>
        /// Parses a hard or soft linebreak.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline handle_newline(Subject subj)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            int nlpos = subj.Position;

            // skip over newline
            subj.Position++;

            // skip spaces at beginning of line
            var len = subj.Length;
            while (subj.Position < len && subj.Buffer[subj.Position] == ' ')
                subj.Position++;

            if (nlpos > 1 && subj.Buffer[nlpos - 1] == ' ' && subj.Buffer[nlpos - 2] == ' ')
            {
                return new Inline(InlineTag.LineBreak)
                {
                    SourcePosition = nlpos - 2,
                    SourceLastPosition = nlpos + 1,
                };
            }
            else
            {
                return new Inline(InlineTag.SoftBreak)
                {
                    SourcePosition = nlpos,
                    SourceLastPosition = nlpos + 1,
                };
            }
        }
    }
}
