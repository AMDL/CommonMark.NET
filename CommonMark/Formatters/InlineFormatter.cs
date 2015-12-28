using CommonMark.Formatters.Inlines;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Inline element formatter.
    /// </summary>
    public class InlineFormatter : ElementFormatter<Inline, InlineTag>, IInlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Inline element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        /// <param name="isSelfClosing"><c>true</c> if <paramref name="htmlTag"/> is self-closing.</param>
        /// <param name="textTag">Text syntax tree tag. If unspecified, <paramref name="htmlTag"/> will be used.</param>
        public InlineFormatter(FormatterParameters parameters, InlineTag tag, string htmlTag = null, bool isSelfClosing = false, string textTag = null)
            : base(parameters, tag, htmlTag, isSelfClosing, textTag)
        {
        }

        /// <summary>
        /// Checks whether the formatter can handle an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public override bool CanHandle(Inline element)
        {
            return element.Tag == Tag;
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public virtual bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            var value = "<" + HtmlTag;
            writer.WriteConstant(value);
            WritePosition(writer, element);
            if (IsSelfClosing)
                writer.WriteConstant(" />");
            else
                writer.Write('>');
            return !IsSelfClosing;
        }

        /// <summary>
        /// Writes the plain text opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public virtual bool WritePlaintextOpening(IHtmlTextWriter writer, Inline element)
        {
            writer.WriteEncodedHtml(element.LiteralContentValue);
            return !IsSelfClosing;
        }

        /// <summary>
        /// Returns the plain text closing of an inline element.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns>The closing.</returns>
        public virtual string GetPlaintextClosing(Inline element)
        {
            return null;
        }

        /// <summary>
        /// Determines whether inline content should be rendered as plain text.
        /// </summary>
        /// <param name="element">Inline element.</param>
        /// <returns><c>true</c> to render the child inlines as plain text.</returns>
        public virtual bool IsPlaintextInlines(Inline element)
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the infix of an inline element.
        /// </summary>
        public string Infix
        {
            get;
            set;
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

        internal static IEnumerable<IInlineFormatter> InitializeFormatters(FormatterParameters parameters)
        {
            yield return new StringFormatter(parameters);
            yield return new LineBreakFormatter(parameters);
            yield return new SoftBreakFormatter(parameters);
            yield return new CodeFormatter(parameters);
            yield return new RawHtmlFormatter(parameters);
            yield return new LinkFormatter(parameters);
            yield return new ImageFormatter(parameters);
            yield return new WeakFormatter(parameters);
            yield return new StrongFormatter(parameters);
        }
    }
}
