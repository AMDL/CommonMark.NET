using System.Collections.Generic;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Base element formatter class.
    /// </summary>
    /// <typeparam name="TElement">Type of element.</typeparam>
    /// <typeparam name="TTag">Type of element tag.</typeparam>
    public abstract class ElementFormatter<TElement, TTag>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementFormatter{TElement,TTag}"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        protected ElementFormatter(FormatterParameters parameters, TTag tag, string htmlTag)
        {
            this.Parameters = parameters;
            this.Tag = tag;
            this.HtmlTag = this.PrinterTag = htmlTag;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the element tag.
        /// </summary>
        /// <value>The element tag handled by this formatter.</value>
        public TTag Tag
        {
            get;
        }

        /// <summary>
        /// Gets the syntax tree node tag.
        /// </summary>
        /// <value>Tag.</value>
        public string PrinterTag
        {
            get;
            protected set;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public abstract bool CanHandle(TElement element);

        /// <summary>
        /// Writes the opening of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        protected bool DoWriteOpening(IHtmlTextWriter writer, TElement element)
        {
            var value = "<" + HtmlTag;
            writer.WriteConstant(value);
            WritePosition(writer, element);
            writer.Write('>');
            return true;
        }

        /// <summary>
        /// Returns the closing of an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>The closing.</returns>
        protected string DoGetClosing(TElement element)
        {
            return "</" + HtmlTag + '>';
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public virtual IDictionary<string, object> GetPrinterData(IPrinter printer, TElement element)
        {
            return null;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Writes the position of an element if position tracking is enabled.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        protected void WritePosition(IHtmlTextWriter writer, TElement element)
        {
            if (Parameters.TrackPositions)
                DoWritePosition(writer, element);
        }

        /// <summary>
        /// Writes the position of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        protected abstract void DoWritePosition(IHtmlTextWriter writer, TElement element);

        /// <summary>
        /// Resolves a URI using <see cref="FormatterParameters.UriResolver"/>.
        /// </summary>
        /// <param name="targetUri">Target URI.</param>
        /// <returns>Resolved URI, or the target URI if the URI resolver is not set.</returns>
        protected string ResolveUri(string targetUri)
        {
            return Parameters.UriResolver != null
                ? Parameters.UriResolver(targetUri)
                : targetUri;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Determines whether the specified object has the same element tag.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if the object has the same element tag.</returns>
        public override bool Equals(object obj)
        {
            var f = obj as ElementFormatter<TElement, TTag>;
            return f != null && f.Tag.Equals(Tag);
        }

        /// <summary>
        /// Returns the hash code of the element tag.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }

        /// <summary>
        /// Returns the element tag name.
        /// </summary>
        /// <returns>Type name.</returns>
        public override string ToString()
        {
            return Tag.ToString();
        }

        #endregion

        private FormatterParameters Parameters
        {
            get;
        }

        private string HtmlTag
        {
            get;
        }
    }
}
