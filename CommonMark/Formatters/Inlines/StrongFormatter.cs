using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.Strong"/> element formatter.
    /// </summary>
    public sealed class StrongFormatter : EmphasisFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrongFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public StrongFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Strong, htmlTags: "strong")
        {
        }
    }
}
