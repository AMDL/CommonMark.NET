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
            : base(parameters, InlineTag.Code, "code", isSelfClosing: false)
        {
        }

        /// <summary>
        /// Returns the inline content rendering option.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        public override bool? IsRenderPlainTextInlines(Inline element)
        {
            return false;
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
