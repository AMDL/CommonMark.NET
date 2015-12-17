using CommonMark.Formatters;
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
        /// Creates the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return Register('~', new InlineDelimiterHandler(doubleCharacterTag: InlineTag.Strikethrough));
        }

        /// <summary>
        /// Creates the mapping from inline tag to inline element formatter.
        /// </summary>
        protected override IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return Register(InlineTag.Strikethrough, new InlineFormatter(parameters, InlineTag.Strikethrough, "del"));
        }
    }
}
