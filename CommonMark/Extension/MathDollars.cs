using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Parser;
using CommonMark.Parser.Inlines.Delimiters;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Single dollars will be interpreted as math delimiters.
    /// The closing dollar must not be followed by a digit.
    /// </summary>
    public class MathDollars : CommonMarkExtension
    {
        /// <summary>
        /// Initializes the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return Register('$', new MathDollarHandler());
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new MathFormatter(parameters);
        }
    }
}
