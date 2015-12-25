using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.Emphasis"/> element formatter.
    /// </summary>
    public sealed class WeakFormatter : EmphasisFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public WeakFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Emphasis, "em", "emph")
        {
        }
    }
}
