using CommonMark.Formatters.Inlines;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Inline element formatter.
    /// </summary>
    public abstract class InlineFormatter : ElementFormatter<Inline>, IInlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected InlineFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        /// <summary>
        /// Returns the content rendering option for an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        public virtual bool? IsRenderPlainTextInlines(Inline inline)
        {
            return null;
        }

        /// <summary>
        /// Writes the position of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="inline">Inline element.</param>
        protected override void DoWritePosition(IHtmlTextWriter writer, Inline inline)
        {
            writer.WritePosition(inline);
        }

        internal static IInlineFormatter[] InitializeFormatters(FormatterParameters parameters)
        {
            var f = new InlineFormatter[(int)InlineTag.Count];
            return f;
        }
    }
}
