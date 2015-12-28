using CommonMark.Formatters;
using CommonMark.Parser;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Single tildes will be interpreted as subscript delimiters.
    /// </summary>
    public class Subscript : CommonMarkExtension
    {
        /// <summary>
        /// Creates the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IEnumerable<IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            yield return new InlineDelimiterHandler('~', InlineTag.Subscript);
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new InlineFormatter(parameters, InlineTag.Subscript, htmlTags: "sub");
        }
    }
}
