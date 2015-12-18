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
            : base(parameters, BlockTag.IndentedCode, printerTag: "indented_code")
        {
        }
    }
}
