using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline stack delimiter parameters.
    /// </summary>
    public struct InlineDelimiterParameters
    {
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
