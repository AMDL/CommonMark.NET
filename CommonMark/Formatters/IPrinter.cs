namespace CommonMark.Formatters
{
    /// <summary>
    /// Syntax tree printer interface.
    /// </summary>
    public interface IPrinter
    {
        /// <summary>
        /// Formats a string.
        /// </summary>
        /// <param name="s">Source string.</param>
        /// <returns>Formatted string.</returns>
        string Format(string s);

        /// <summary>
        /// Formats a string content.
        /// </summary>
        /// <param name="stringContent">String content.</param>
        /// <returns>Formatted string.</returns>
        string Format(Syntax.StringContent stringContent);
    }
}
