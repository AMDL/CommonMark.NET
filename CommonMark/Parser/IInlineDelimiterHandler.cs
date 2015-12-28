namespace CommonMark.Parser
{
    /// <summary>
    /// Inline delimiter handler.
    /// </summary>
    public interface IInlineDelimiterHandler
    {
        /// <summary>
        /// Gets the handled character.
        /// </summary>
        char Character
        {
            get;
        }

        /// <summary>
        /// Returns the inline element tag that corresponds to the specified delimiter count.
        /// </summary>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <returns>Tag or 0.</returns>
        Syntax.InlineTag GetTag(int delimiterCount);

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
        bool IsCanOpen(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canClose);

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
        bool IsCanClose(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canOpen);
    }
}
