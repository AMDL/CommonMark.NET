using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Single <c>&lt;</c> parser.
    /// </summary>
    public class LessThanParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LessThanParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public LessThanParser(CommonMarkSettings settings)
            : base(settings, '<')
        {
        }

        /// <summary>
        /// Handles an element.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        public override Inline Handle(Block container, Subject subj)
        {
            // advance past the opening <
            subj.Position++;

            // just return the opening <
            return new Inline("<", subj.Position - 1, subj.Position);
        }
    }
}
