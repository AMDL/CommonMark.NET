using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Base inline delimiter handler class.
    /// </summary>
    public class InlineDelimiterHandler : IInlineDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDelimiterHandler"/> class.
        /// </summary>
        /// <param name="character">Handled character.</param>
        /// <param name="singleCharacterTag">The tag to use for inline elements created when a single-character delimiter is matched.</param>
        /// <param name="doubleCharacterTag">The tag to use for inline elements created when a double-character delimiter is matched.</param>
        public InlineDelimiterHandler(char character, InlineTag singleCharacterTag = 0, InlineTag doubleCharacterTag = 0)
        {
            Character = character;
            SingleCharacterTag = singleCharacterTag;
            DoubleCharacterTag = doubleCharacterTag;
        }

        /// <summary>
        /// Returns the inline element tag that corresponds to the specified delimiter count.
        /// </summary>
        /// <param name="delimiterCount">Delimiter character count.</param>
        /// <returns>Tag or 0.</returns>
        public virtual InlineTag GetTag(int delimiterCount)
        {
            switch (delimiterCount)
            {
                case 1:
                    return SingleCharacterTag;
                case 2:
                    return DoubleCharacterTag;
                default:
                    return 0;
            }
        }

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

        /// <summary>
        /// Gets the handled character.
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// Gets the single-character element tag.
        /// </summary>
        /// <value>The tag used for inline elements created when a single-character delimiter is matched.</value>
        protected InlineTag SingleCharacterTag { get; }

        /// <summary>
        /// Gets the double-character element tag.
        /// </summary>
        /// <value>The tag used for inline elements created when a double-character delimiter is matched.</value>
        protected InlineTag DoubleCharacterTag { get; }
    }
}
