using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Code element parser.
    /// </summary>
    public class CodeParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public CodeParser(CommonMarkSettings settings)
            : base(settings, '`')
        {
        }

        /// <summary>
        /// Parses backtick code section or raw backticks.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline handle_backticks(Subject subj)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            int ticklength = 0;
            var bl = subj.Length;
            while (subj.Position < bl && (subj.Buffer[subj.Position] == '`'))
            {
                ticklength++;
                subj.Position++;
            }

            int startpos = subj.Position;
            int endpos = ScanToClosingBackticks(subj, ticklength);
            if (endpos == 0)
            {
                // closing not found
                subj.Position = startpos; // rewind to right after the opening ticks
                return new Inline(new string('`', ticklength), startpos - ticklength, startpos);
            }
            else
            {
                return new Inline(InlineTag.Code, InlineMethods.NormalizeWhitespace(subj.Buffer, startpos, endpos - startpos - ticklength))
                {
                    SourcePosition = startpos - ticklength,
                    SourceLastPosition = endpos
                };
            }
        }

        /// <summary>
        /// Searches the subject for a span of backticks that matches the given length.
        /// Also updates the position on the subject itself.
        /// </summary>
        /// <returns><c>0</c> if the closing backticks cannot be found, or
        /// the position in the subject after the closing backticks</returns>
        private static int ScanToClosingBackticks(Subject subj, int openticklength)
        {
            // note - attempt to optimize by using string.IndexOf("````",...) proved to
            // be ~2x times slower than the current implementation.
            // but - buf.IndexOf('`') gives ~1.5x better performance than iterating over
            // every char in the loop.

            var buf = subj.Buffer;
            var len = buf.Length;
            var cc = 0;

            for (var i = subj.Position; i < len; i++)
            {
                if (buf[i] == '`')
                {
                    cc++;
                }
                else
                {
                    if (cc == openticklength)
                        return subj.Position = i;

                    i = buf.IndexOf('`', i, len - i) - 1;
                    if (i == -2)
                        return 0;

                    cc = 0;
                }
            }

            if (cc == openticklength)
                return subj.Position = len;

            return 0;
        }
    }
}
