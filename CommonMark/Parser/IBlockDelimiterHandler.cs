namespace CommonMark.Parser
{
    /// <summary>
    /// Block delimiter handler.
    /// </summary>
    public interface IBlockDelimiterHandler
    {
        /// <summary>
        /// Gets the handled character.
        /// </summary>
        /// <value>A character that can open a handled element.</value>
        char Character { get; }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        bool Handle(ref BlockParserInfo info);
    }
}
