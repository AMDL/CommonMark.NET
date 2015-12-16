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
        public AsteriskParser(InlineParserParameters parameters)
            : base(InlineTag.Emphasis, InlineTag.Strong, parameters, '*')
        {
        }
    }
}
