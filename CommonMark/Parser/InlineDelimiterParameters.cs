﻿using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline stack delimiter matcher delegate.
    /// </summary>
    /// <param name="subj">The source subject.</param>
    /// <param name="startIndex">The index of the first character.</param>
    /// <param name="length">The length of the substring.</param>
    /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
    /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
    /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
    /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
    public delegate void InlineDelimiterMatcherDelegate(Subject subj, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose);

    /// <summary>
    /// Inline stack delimiter parameters.
    /// </summary>
    public struct InlineDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDelimiterParameters"/> structure.
        /// </summary>
        /// <param name="tag">The tag to use for the inline element when the opener is matched.</param>
        public InlineDelimiterParameters(InlineTag tag)
        {
            this.Tag = tag;
            this.Matcher = null;
        }

        /// <summary>
        /// The tag to use for the inline element when the opener is matched.
        /// </summary>
        public InlineTag Tag;

        /// <summary>
        /// Matcher delegate.
        /// </summary>
        public InlineDelimiterMatcherDelegate Matcher;

        /// <summary>
        /// Determines whether the parameters instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return Tag == 0; }
        }
    }

    /// <summary>
    /// Inline stack delimiter character parameters.
    /// </summary>
    public struct InlineDelimiterCharacterParameters
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
    }
}