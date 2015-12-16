namespace CommonMark.Parser
{
    /// <summary>
    /// Inline element parser.
    /// </summary>
    public interface IInlineParser
    {
        /// <summary>
        /// Gets the opening character handled by this parser.
        /// </summary>
        /// <value>Character that can open a handled element.</value>
        char Character { get; }

        /// <summary>
        /// Handles an element.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subject">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        Syntax.Inline Handle(Syntax.Block container, Subject subject);
    }
}
