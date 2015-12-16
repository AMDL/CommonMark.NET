using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Underscore emphasis parser.
    /// </summary>
    public class UnderscoreParser : EmphasisParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderscoreParser"/> class.
        /// </summary>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <param name="settings">Common settings.</param>
        public UnderscoreParser(InlineParserParameters parameters, CommonMarkSettings settings)
            : base(InlineTag.Emphasis, InlineTag.Strong, parameters, settings, '_')
        {
        }

        /// <summary>
        /// Attempts to match a stack delimiter.
        /// </summary>
        /// <param name="subj">The source subject.</param>
        /// <param name="startpos">The index of the first character.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
        /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
        /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
        /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
        protected override void Match(Subject subj, int startpos, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            var temp = canOpen;
            canOpen &= (!canClose || beforeIsPunctuation);
            canClose &= (!temp || afterIsPunctuation);
        }
    }
}
