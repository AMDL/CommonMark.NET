using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    /// <summary>
    /// <see cref="InlineTag.Image"/> element formatter.
    /// </summary>
    public sealed class ImageFormatter : InlineFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        public ImageFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Image, textTag: "image", htmlTags: "img")
        {
            Infix = "\" title=\"";
            IsSelfClosing = true;
            IsFixedHtmlInlines = true;
            IsFixedPlaintextInlines = true;
        }

        /// <summary>
        /// Writes the opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline lement.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            StartWriteOpening(writer, element);
            writer.WriteConstant(" src=\"");
            writer.WriteEncodedUrl(ResolveUri(element.TargetUrl));
            writer.WriteConstant("\" alt=\"");
            return false;
        }

        /// <summary>
        /// Writes the plaintext opening of an inline element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Inline lement.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WritePlaintextOpening(IHtmlTextWriter writer, Inline element)
        {
            return true;
        }

        /// <summary>
        /// Returns the closing of an inline element.
        /// </summary>
        /// <param name="element">Inline lement.</param>
        /// <returns>The closing.</returns>
        public override string GetClosing(Inline element)
        {
            return "\" />";
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Inline element)
        {
            return new Dictionary<string, object>
            {
                { "url", formatter.Format(element.TargetUrl) },
                { "title", formatter.Format(element.LiteralContent) },
            };
        }
    }
}
