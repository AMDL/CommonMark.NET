using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Link opener parser.
    /// </summary>
    public class LinkOpenerParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkOpenerParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public LinkOpenerParser(CommonMarkSettings settings)
            : base('[', settings)
        {
        }

        /// <summary>
        /// Parses a left bracket.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline HandleLeftSquareBracket(Subject subj</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            return HandleLeftBracket(subj, "[", InlineStack.InlineStackFlags.Opener, subj.Position);
        }
    }
}
