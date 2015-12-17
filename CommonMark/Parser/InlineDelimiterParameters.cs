using CommonMark.Syntax;
using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline stack delimiter match handler delegate.
    /// </summary>
    /// <param name="subj">The source subject.</param>
    /// <param name="delimiterCount">Delimiter character count.</param>
    /// <param name="startIndex">The index of the first character.</param>
    /// <param name="length">The length of the substring.</param>
    /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
    /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
    /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
    /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool InlineDelimiterHandlerDelegate(Subject subj, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose);

    /// <summary>
    /// Inline stack delimiter parameters.
    /// </summary>
    internal struct InlineDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDelimiterParameters"/> structure.
        /// </summary>
        /// <param name="tag">The tag to use for the inline element when the opener is matched.</param>
        public InlineDelimiterParameters(InlineTag tag)
        {
            this.Tag = tag;
        }

        /// <summary>
        /// The tag to use for the inline element when the opener is matched.
        /// </summary>
        public InlineTag Tag;

        /// <summary>
        /// Determines whether the parameters instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return Tag == 0; }
        }

        /// <summary>
        /// Merges two parameters objects.
        /// </summary>
        /// <param name="params1">First parameters object.</param>
        /// <param name="params2">Second parameters object.</param>
        /// <param name="key">Identifier to use in an exception message.</param>
        /// <returns>Merged parameters object.</returns>
        /// <exception cref="InvalidOperationException">Both objects are non-empty.</exception>
        internal static InlineDelimiterParameters Merge(InlineDelimiterParameters params1, InlineDelimiterParameters params2, string key)
        {
            if (!params1.IsEmpty && !params2.IsEmpty)
                throw new InvalidOperationException(string.Format("{0} parameters value is already set: {1}.", key, params1));
            return !params1.IsEmpty ? params1 : params2;
        }
    }

    /// <summary>
    /// Inline stack delimiter character parameters.
    /// </summary>
    internal struct InlineDelimiterCharacterParameters
    {
        /// <summary>
        /// Determines whether the parameters instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return SingleCharacter.IsEmpty && DoubleCharacter.IsEmpty; }
        }

        /// <summary>
        /// Single character delimiter parameters.
        /// </summary>
        public InlineDelimiterParameters SingleCharacter;

        /// <summary>
        /// Double character delimiter parameters.
        /// </summary>
        public InlineDelimiterParameters DoubleCharacter;

        /// <summary>
        /// Handler delegate.
        /// </summary>
        public InlineDelimiterHandlerDelegate Handler;

        /// <summary>
        /// Merges two parameters objects.
        /// </summary>
        /// <param name="params1">First parameters object.</param>
        /// <param name="params2">Second parameters object.</param>
        /// <returns>Merged parameters object.</returns>
        /// <exception cref="InvalidOperationException">Child parameters of the same kind are non-empty in both objects.</exception>
        internal static InlineDelimiterCharacterParameters Merge(InlineDelimiterCharacterParameters params1, InlineDelimiterCharacterParameters params2)
        {
            return new InlineDelimiterCharacterParameters
            {
                SingleCharacter = InlineDelimiterParameters.Merge(params1.SingleCharacter, params2.SingleCharacter, "Single character"),
                DoubleCharacter = InlineDelimiterParameters.Merge(params1.DoubleCharacter, params2.DoubleCharacter, "Double character"),
                Handler = DelegateInlineDelimiterMatcher.Merge(params1.Handler, params2.Handler),
            };
        }
    }
}
