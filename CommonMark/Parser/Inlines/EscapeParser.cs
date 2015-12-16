using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Backslash escape parser.
    /// </summary>
    public class EscapeParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public EscapeParser(CommonMarkSettings settings)
            : base('\\', settings)
        {
        }

        /// <summary>
        /// Parses backslash-escape or just a backslash.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline handle_backslash(Subject subj)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            subj.Position++;

            if (subj.Position >= subj.Length)
                return new Inline("\\", subj.Position - 1, subj.Position);

            var nextChar = subj.Buffer[subj.Position];

            if (Utilities.IsEscapableSymbol(nextChar))
            {
                // only ascii symbols and newline can be escaped
                // the exception is the unicode bullet char since it can be used for defining list items
                subj.Position++;
                return new Inline(nextChar.ToString(), subj.Position - 2, subj.Position);
            }
            else if (nextChar == '\n')
            {
                subj.Position++;
                return new Inline(InlineTag.LineBreak)
                {
                    SourcePosition = subj.Position - 2,
                    SourceLastPosition = subj.Position
                };
            }
            else
            {
                return new Inline("\\", subj.Position - 1, subj.Position);
            }
        }
    }
}
