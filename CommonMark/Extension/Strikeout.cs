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
        private static readonly InlineDelimiterParameters DoubleChar = InitializeDoubleChar();

        private readonly StrikethroughFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Strikeout"/> class.
        /// </summary>
        /// <param name="settings">The object containing settings to be cloned for the formatting process.</param>
        public Strikeout(CommonMarkSettings settings)
        {
            this.formatter = new StrikethroughFormatter(settings.Clone());
        }

        /// <summary>
        /// Gets the mapping from character to inline delimiter parameters for matched double-character openers.
        /// </summary>
        public override IDictionary<char, InlineDelimiterParameters> InlineDoubleChars
        {
            get
            {
                return new Dictionary<char, InlineDelimiterParameters>
                {
                    { '~', DoubleChar }
                };
            }
        }

        /// <summary>
        /// Gets the mapping from inline tag to inline element formatter.
        /// </summary>
        public override IDictionary<InlineTag, IInlineFormatter> InlineFormatters
        {
            get
            {
                return new Dictionary<InlineTag, IInlineFormatter>
                {
                    { InlineTag.Strikethrough, formatter }
                };
            }
        }

        private static InlineDelimiterParameters InitializeDoubleChar()
        {
            return new InlineDelimiterParameters
            {
                Tag = InlineTag.Strikethrough,
            };
        }
    }
}
