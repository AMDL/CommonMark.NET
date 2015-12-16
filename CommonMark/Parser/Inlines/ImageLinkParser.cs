using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Image link parser.
    /// </summary>
    public class ImageLinkParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLinkParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public ImageLinkParser(CommonMarkSettings settings)
            : base('!', settings)
        {
        }

        /// <summary>
        /// Parses an image link or just an exclamation mark.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline handle_exclamation(Subject subj)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            subj.Position++;

            if (InlineMethods.peek_char(subj) == '[')
                return HandleLeftBracket(subj, "![", InlineStack.InlineStackFlags.Opener | InlineStack.InlineStackFlags.ImageLink, subj.Position - 1);
            else
                return new Inline("!", subj.Position - 1, subj.Position);
        }
    }
}
