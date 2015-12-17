using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Parser;
using CommonMark.Parser.Inlines.Delimiters;
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
        /// Initializes a new instance of the <see cref="Subscript"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public Subscript(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Creates the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return Register('~', new SubscriptTildeHandler());
        }

        /// <summary>
        /// Creates the mapping from inline tag to inline element formatter.
        /// </summary>
        protected override IDictionary<InlineTag, IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return Register(InlineTag.Subscript, new SubscriptFormatter(parameters));
        }
    }
}
