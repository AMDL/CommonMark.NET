namespace CommonMark.Parser
{
    /// <summary>
    /// Block delimiter handler.
    /// </summary>
    public interface IBlockDelimiterHandler
    {
        /// <summary>
        /// Gets the handled characters.
        /// </summary>
        /// <value>The characters that can open a handled element.</value>
        char[] Characters { get; }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        bool Handle(ref BlockParserInfo info);
    }
}
