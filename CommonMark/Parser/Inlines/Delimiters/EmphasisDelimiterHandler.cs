namespace CommonMark.Parser.Inlines.Delimiters
{
    /// <summary>
    /// Base emphasis delimiter handler class.
    /// </summary>
    public abstract class EmphasisHandler : InlineDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisHandler"/> class.
        /// </summary>
        protected EmphasisHandler(char character)
            : base(character, Syntax.InlineTag.Emphasis, Syntax.InlineTag.Strong)
        {
        }
    }

    /// <summary>
    /// Asterisk handler.
    /// </summary>
    public class AsteriskHandler : EmphasisHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsteriskHandler"/> class.
        /// </summary>
        public AsteriskHandler()
            : base('*')
        {
        }
    }

    /// <summary>
    /// Underscore handler.
    /// </summary>
    public class UnderscoreHandler : EmphasisHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderscoreHandler"/> class.
        /// </summary>
        public UnderscoreHandler()
            : base('_')
        {
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
