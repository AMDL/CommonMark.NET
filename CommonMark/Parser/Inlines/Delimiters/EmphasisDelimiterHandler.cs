using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    /// <summary>
    /// Base emphasis delimiter handler class.
    /// </summary>
    public abstract class EmphasisHandler : InlineDelimiterHandler
    {
        /// <summary>
        /// Returns the inline element tag that corresponds to the specified delimiter count.
        /// </summary>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <returns>Tag or 0.</returns>
        public override InlineTag GetTag(int delimiterCount)
        {
            if (delimiterCount == 1)
                return InlineTag.Emphasis;
            if (delimiterCount == 2)
                return InlineTag.Strong;
            return 0;
        }

        /// <summary>
        /// Handles a matched inline stack delimiter.
        /// </summary>
        /// <param name="subject">The source subject.</param>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <param name="startIndex">The index of the first character.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
        /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
        /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
        /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(Subject subject, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            return delimiterCount <= 2;
        }
    }

    /// <summary>
    /// Asterisk handler.
    /// </summary>
    public class AsteriskHandler : EmphasisHandler
    {
    }

    /// <summary>
    /// Underscore handler.
    /// </summary>
    public class UnderscoreHandler : EmphasisHandler
    {
        /// <summary>
        /// Handles a matched inline stack delimiter.
        /// </summary>
        /// <param name="subject">The source subject.</param>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <param name="startIndex">The index of the first character.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
        /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
        /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
        /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(Subject subject, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            if (!base.Handle(subject, delimiterCount, startIndex, length, beforeIsPunctuation, afterIsPunctuation, ref canOpen, ref canClose))
                return false;
            var temp = canOpen;
            canOpen &= (!canClose || beforeIsPunctuation);
            canClose &= (!temp || afterIsPunctuation);
            return true;
        }
    }
}
