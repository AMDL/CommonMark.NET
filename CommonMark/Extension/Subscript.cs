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
            return Register('~', new InlineDelimiterHandler(InlineTag.Subscript));
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new InlineFormatter(parameters, InlineTag.Subscript, "sub");
        }
    }
}
