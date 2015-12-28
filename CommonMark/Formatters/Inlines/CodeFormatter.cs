using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.Code"/> element formatter.
    /// </summary>
    public sealed class CodeFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public CodeFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Code, htmlTags: "code")
        {
            IsFixedOpening = true;
        }

        /// <summary>
        /// Determines whether inline content should be rendered as HTML.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> to render the literal content as HTML.</returns>
        public override bool IsHtmlInlines(Inline element)
        {
            return true;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Inline element)
        {
            return new Dictionary<string, object>
            {
                { string.Empty, formatter.Format(element.LiteralContent) },
            };
        }
    }
}
