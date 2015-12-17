namespace CommonMark.Parser
{
    /// <summary>
    /// Inline delimiter handler.
    /// </summary>
    public interface IInlineDelimiterHandler
    {
        /// <summary>
        /// Returns the inline element tag that corresponds to the specified delimiter count.
        /// </summary>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <returns>Tag or 0.</returns>
        Syntax.InlineTag GetTag(int delimiterCount);

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
        bool Handle(Subject subject, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose);
    }
}
