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
        /// <param name="textTag">Text syntax tree tag. If unspecified, the first element of <paramref name="htmlTags"/> will be used.</param>
        /// <param name="htmlTags">HTML tags.</param>
        public InlineFormatter(FormatterParameters parameters, InlineTag tag, string textTag = null, params string[] htmlTags)
            : base(parameters, tag, textTag, htmlTags)
        {
            IsFixedPlaintextInlines = false;
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
            StartWriteOpening(writer, element);
            if (IsSelfClosing)
                writer.WriteConstant(" />");
            else
                writer.Write('>');
            return !IsSelfClosing;
        }

        /// <summary>
        /// Writes the start of an inline element opening.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline element.</param>
        protected void StartWriteOpening(IHtmlTextWriter writer, Inline element)
        {
            var value = string.Empty;
            for (int i = 0; i < HtmlTags.Length; i++)
            {
                if (value.Length > 0)
                    value += '>';
                value += "<" + HtmlTags[i];
            }
            writer.WriteConstant(value);
            WritePosition(writer, element);
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
            return IsFixedPlaintextInlines == true;
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
        /// Gets a value indicating whether inline content should always be rendered as plain text.
        /// </summary>
        /// <value><c>true</c> to render the child inlines as plain text.</value>
        public bool? IsFixedPlaintextInlines
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
