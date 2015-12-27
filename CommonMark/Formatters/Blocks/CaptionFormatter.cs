using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Caption element formatter.
    /// </summary>
    public class CaptionFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptionFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        public CaptionFormatter(FormatterParameters parameters, BlockTag tag, string htmlTag)
            : base(parameters, tag, htmlTag)
        {
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block element)
        {
            return new Dictionary<string, object>
            {
                { "placement", element.Caption.Placement },
                { "lead", element.Caption.Lead },
            };
        }
    }
}
