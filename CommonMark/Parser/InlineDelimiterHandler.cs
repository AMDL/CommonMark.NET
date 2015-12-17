namespace CommonMark.Parser
{
    /// <summary>
    /// Base inline delimiter handler class.
    /// </summary>
    public abstract class InlineDelimiterHandler : IInlineDelimiterHandler
    {
        /// <summary>
        /// Returns the inline element tag that corresponds to the specified delimiter count.
        /// </summary>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <returns>Tag or 0.</returns>
        public abstract Syntax.InlineTag GetTag(int delimiterCount);

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
        public virtual bool IsCanOpen(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canClose)
        {
            return true;
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
        public virtual bool IsCanClose(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canOpen)
        {
            return true;
        }
    }
}
