using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.IndentedCode"/> element formatter.
    /// </summary>
    public class IndentedCodeFormatter : CodeBlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedCodeFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public IndentedCodeFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public override bool CanHandle(Block element)
        {
            return element.Tag == BlockTag.IndentedCode;
        }

        /// <summary>
        /// Returns the syntax tree node tag for an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        public override string GetPrinterTag(Block element)
        {
            return "indented_code";
        }
    }
}
