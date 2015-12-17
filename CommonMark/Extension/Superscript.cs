using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Parser;
using CommonMark.Parser.Inlines.Delimiters;
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
        /// Initializes a new instance of the <see cref="Superscript"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public Superscript(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Creates the mapping from character to inline delimiter handler.
        /// </summary>
        protected override IDictionary<char, IInlineDelimiterHandler> InitializeInlineDelimiterHandlers()
        {
            return Register('^', new SuperscriptCaretHandler());
        }

        /// <summary>
        /// Creates the mapping from inline tag to inline element formatter.
        /// </summary>
        protected override IDictionary<InlineTag, Formatters.IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            return Register(InlineTag.Superscript, new SuperscriptFormatter(parameters));
        }
    }
}
