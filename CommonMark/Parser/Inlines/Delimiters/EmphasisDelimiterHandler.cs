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
        /// Handles a matched opener.
        /// </summary>
        /// <param name="subject">The source subject.</param>
        /// <param name="startIndex">The index of the first character.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="before">The type of the preceding character.</param>
        /// <param name="after">The type of the following character.</param>
        /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
        /// <returns><c>true</c> if the delimiter can serve as an opener.</returns>
        public override bool IsCanOpen(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canClose)
        {
            return !canClose || before.IsPunctuation;
        }

        /// <summary>
        /// Handles a matched closer.
        /// </summary>
        /// <param name="subject">The source subject.</param>
        /// <param name="startIndex">The index of the first character.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="before">The type of the preceding character.</param>
        /// <param name="after">The type of the following character.</param>
        /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
        /// <returns><c>true</c> if the delimiter can serve as a closer.</returns>
        public override bool IsCanClose(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canOpen)
        {
            return !canOpen || after.IsPunctuation;
        }
    }
}
