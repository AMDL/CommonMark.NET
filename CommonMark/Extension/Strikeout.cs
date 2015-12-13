using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Parser;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Double tildes will be interpreted as strikeout markers.
    /// </summary>
    public class Strikeout : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Strikeout"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public Strikeout(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Creates the mapping from character to inline delimiter character parameters.
        /// </summary>
        protected override IDictionary<char, InlineDelimiterCharacterParameters> InitializeInlineDelimiterCharacters()
        {
            return new Dictionary<char, InlineDelimiterCharacterParameters>
            {
                { '~', InitializeDelimiters() }
            };
        }

        /// <summary>
        /// Creates the mapping from inline tag to inline element formatter.
        /// </summary>
        protected override IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return new Dictionary<InlineTag, IInlineFormatter>
            {
                { InlineTag.Strikethrough, new StrikethroughFormatter(parameters) }
            };
        }

        private static InlineDelimiterCharacterParameters InitializeDelimiters()
        {
            return new InlineDelimiterCharacterParameters
            {
                DoubleCharacter = InitializeDoubleCharacter()
            };
        }

        private static InlineDelimiterParameters InitializeDoubleCharacter()
        {
            return new InlineDelimiterParameters
            {
                Tag = InlineTag.Strikethrough,
            };
        }
    }
}
