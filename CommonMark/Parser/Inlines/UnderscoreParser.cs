using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Underscore emphasis parser.
    /// </summary>
    public class UnderscoreParser : EmphasisParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderscoreParser"/> class.
        /// </summary>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <param name="settings">Common settings.</param>
        public UnderscoreParser(InlineParserParameters parameters, CommonMarkSettings settings)
            : base('_', InlineTag.Emphasis, InlineTag.Strong, parameters, settings)
        {
        }
    }
}
