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
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public virtual bool WriteOpening(IHtmlTextWriter writer, Inline element, bool withinLink)
        {
            return base.WriteOpening(writer, element);
        }

        /// <summary>
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's link stacking option.</param>
        /// <returns>The closing.</returns>
        public virtual string GetClosing(IHtmlFormatter formatter, Inline element, bool withinLink)
        {
            return base.GetClosing(formatter, element);
        }

        /// <summary>
        /// Returns the content rendering option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="plaintext">The parent's rendering option.</param>
        /// <returns>
        /// <c>true</c> to render the child inlines as plain text,
        /// <c>false</c> to render the literal content as HTML,
        /// or <c>null</c> to skip content rendering.
        /// </returns>
        public virtual bool? IsRenderPlainTextInlines(Inline element, bool plaintext)
        {
            return null;
        }

        /// <summary>
        /// Returns the link stacking option for an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <param name="withinLink">The parent's stacking option.</param>
        /// <returns><c>true</c> to stack elements within a link.</returns>
        public virtual bool IsStackWithinLink(Inline element, bool withinLink)
        {
            return false;
        }

        /// <summary>
        /// Writes the position of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        protected override void DoWritePosition(IHtmlTextWriter writer, Inline element)
        {
            writer.WritePosition(element);
        }

        internal static IInlineFormatter[] InitializeFormatters(FormatterParameters parameters)
        {
            var f = new InlineFormatter[(int)InlineTag.Count];
            f[(int)InlineTag.Link] = new LinkFormatter(parameters);
            return f;
        }
    }
}
