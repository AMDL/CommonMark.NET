using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Parser;
using CommonMark.Parser.Inlines.Delimiters;
using CommonMark.Syntax;
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
        /// Initializes a new instance of the <see cref="MathDollars"/> class.
        /// </summary>
        /// <param name="settings"></param>
        public MathDollars(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return Register('$', new MathDollarHandler());
        }

        /// <summary>
        /// Initializes the mapping from inline tag to inline element formatter.
        /// </summary>
        protected override IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return Register(InlineTag.Math, new MathFormatter(parameters));
        }
    }
}
