using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// Base emphasis element formatter class.
    /// </summary>
    public abstract class EmphasisFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Inline element tag.</param>
        /// <param name="textTag">Text syntax tree tag. If unspecified, the first element of <paramref name="htmlTags"/> will be used.</param>
        /// <param name="htmlTags">HTML tags.</param>
        protected EmphasisFormatter(FormatterParameters parameters, InlineTag tag, string textTag = null, params string[] htmlTags)
            : base(parameters, tag, textTag, htmlTags)
        {
            IsFixedOpening = true;
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
                { "delim", element.Emphasis.DelimiterCharacter },
            };
        }
    }
}
