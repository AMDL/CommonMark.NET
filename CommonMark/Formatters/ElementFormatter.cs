﻿using System.Collections.Generic;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Base element formatter class.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public abstract class ElementFormatter<T>
    {
        #region Fields

        private readonly FormatterParameters parameters;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementFormatter{T}"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected ElementFormatter(FormatterParameters parameters)
        {
            this.parameters = parameters;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks whether the formatter can handle an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public abstract bool CanHandle(T element);

        /// <summary>
        /// Writes the opening of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        protected bool DoWriteOpening(IHtmlTextWriter writer, T element)
        {
            var value = "<" + GetTag(element);
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
        protected string DoGetClosing(T element)
        {
            return "</" + GetTag(element) + '>';
        }

        /// <summary>
        /// Returns the syntax tree node tag for an element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        public virtual string GetPrinterTag(T element)
        {
            return GetTag(element);
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public virtual IDictionary<string, object> GetPrinterData(IPrinter printer, T element)
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
        protected void WritePosition(IHtmlTextWriter writer, T element)
        {
            if (parameters.TrackPositions)
                DoWritePosition(writer, element);
        }

        /// <summary>
        /// Writes the position of an element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Element.</param>
        protected abstract void DoWritePosition(IHtmlTextWriter writer, T element);

        /// <summary>
        /// Gets the HTML tag for the element.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Tag.</returns>
        protected abstract string GetTag(T element);

        /// <summary>
        /// Resolves a URI using <see cref="FormatterParameters.UriResolver"/>.
        /// </summary>
        /// <param name="targetUri">Target URI.</param>
        /// <returns>Resolved URI, or the target URI if the URI resolver is not set.</returns>
        protected string ResolveUri(string targetUri)
        {
            return parameters.UriResolver != null
                ? parameters.UriResolver(targetUri)
                : targetUri;
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Determines whether the specified object has the same type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if the object is an instance of the same type.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && GetType().Equals(obj.GetType());
        }

        /// <summary>
        /// Returns the hash code of the type object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        /// <summary>
        /// Returns the type name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        #endregion
    }
}
