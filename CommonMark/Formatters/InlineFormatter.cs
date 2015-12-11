using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Inline element formatter.
    /// </summary>
    public abstract class InlineFormatter : IInlineFormatter
    {
        private readonly CommonMarkSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineFormatter"/> class.
        /// </summary>
        /// <param name="settings"></param>
        protected InlineFormatter(CommonMarkSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Checks whether the formatter can handle an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="inline"/>.</returns>
        public abstract bool CanHandle(Inline inline);

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="inline">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child inline elements.</returns>
        public abstract bool WriteOpening(IHtmlTextWriter writer, Inline inline);

        /// <summary>
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>The closing.</returns>
        public abstract string GetClosing(Inline inline);

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
        /// Writes the position of an inline element if position tracking is enabled.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="inline">Inline element.</param>
        protected void WritePosition(IHtmlTextWriter writer, Inline inline)
        {
            if (settings.TrackSourcePosition)
                writer.WritePosition(inline);
        }

        internal static IInlineFormatter[] InitializeFormatters(CommonMarkSettings settings)
        {
            var f = new InlineFormatter[(int)InlineTag.Count];
            return f;
        }

        /// <summary>
        /// Returns the syntax tree node tag for an inline element.
        /// </summary>
        /// <param name="inline">Inline element.</param>
        /// <returns>Tag.</returns>
        public abstract string GetNodeTag(Inline inline);

        /// <summary>
        /// Writes the properties of an inline element.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="inline">Inline element.</param>
        public abstract void Print(TextWriter writer, Inline inline);
    }
}
