using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Double tildes will be interpreted as strikeout markers.
    /// </summary>
    public class Strikeout : CommonMarkExtension
    {
        private readonly StrikethroughFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Strikeout"/> class.
        /// </summary>
        /// <param name="settings">The object containing settings for the formatting process.</param>
        public Strikeout(CommonMarkSettings settings)
        {
            this.formatter = new StrikethroughFormatter(settings.FormatterParameters);
        }

        /// <summary>
        /// Gets the mapping from character to inline tag for matched double-character openers.
        /// </summary>
        public override IDictionary<char, InlineTag> DoubleCharTags
        {
            get
            {
                return new Dictionary<char, InlineTag>
                {
                    { '~', InlineTag.Strikethrough }
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
    }
}
