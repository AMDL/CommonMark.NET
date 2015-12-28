using CommonMark.Formatters;
using CommonMark.Parser;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Single carets will be interpreted as superscript delimiters.
    /// </summary>
    public class Superscript : CommonMarkExtension
    {
        /// <summary>
        /// Creates the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return Register('^', new InlineDelimiterHandler(InlineTag.Superscript));
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new InlineFormatter(parameters, InlineTag.Superscript, "sup");
        }
    }
}
