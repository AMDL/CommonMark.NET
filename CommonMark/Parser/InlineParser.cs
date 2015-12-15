namespace CommonMark.Parser
{
    /// <summary>
    /// Base inline element parser class.
    /// </summary>
    public abstract class InlineParser : ElementParser
    {
        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public abstract char[] Characters
        {
            get;
        }
    }
}
