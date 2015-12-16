using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Asterisk emphasis parser.
    /// </summary>
    public class AsteriskParser : EmphasisParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsteriskParser"/> class.
        /// </summary>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <param name="settings">Common settings.</param>
        public AsteriskParser(InlineParserParameters parameters, CommonMarkSettings settings)
            : base(InlineTag.Emphasis, InlineTag.Strong, parameters, settings, '*')
        {
        }
    }
}
