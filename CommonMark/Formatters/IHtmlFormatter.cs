using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// HTML formatter interface.
    /// </summary>
    public interface IHtmlFormatter
    {
        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        string EscapeHtml(StringPart part);

        /// <summary>
        /// Escapes special URL characters.
        /// </summary>
        string EscapeUrl(string url);

        /// <summary>
        /// Prints inline element position.
        /// </summary>
        string PrintPosition(Inline element);
    }
}
