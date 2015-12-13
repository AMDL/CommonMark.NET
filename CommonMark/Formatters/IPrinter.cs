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
    }
}
