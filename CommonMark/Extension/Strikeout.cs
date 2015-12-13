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

        /// <summary>
        /// Initializes a new instance of the <see cref="Strikeout"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public Strikeout(CommonMarkSettings settings)
            : base(settings)
        {
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
        /// Creates the mapping from inline tag to inline element formatter.
        /// </summary>
        protected override IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return new Dictionary<InlineTag, IInlineFormatter>
            {
                { InlineTag.Strikethrough, new StrikethroughFormatter(parameters) }
            };
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
