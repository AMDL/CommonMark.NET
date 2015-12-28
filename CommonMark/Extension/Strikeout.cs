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
        /// Creates the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IEnumerable<IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            yield return new InlineDelimiterHandler('~', doubleCharacterTag: InlineTag.Strikethrough);
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new InlineFormatter(parameters, InlineTag.Strikethrough, htmlTags: "del");
        }
    }
}
