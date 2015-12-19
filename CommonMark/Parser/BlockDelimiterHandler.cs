namespace CommonMark.Parser
{
    /// <summary>
    /// Base block delimiter handler class.
    /// </summary>
    public abstract class BlockDelimiterHandler : IBlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDelimiterHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        protected BlockDelimiterHandler(CommonMarkSettings settings, char character)
        {
            this.Settings = settings;
            this.Character = character;
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public char Character
        {
            get;
        }

        /// <summary>
        /// Gets the common settings object.
        /// </summary>
        protected CommonMarkSettings Settings
        {
            get;
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public virtual bool Handle(ref BlockParserInfo info)
        {
            return false;
        }

        /// <summary>
        /// Scans a horizontal rule line: "...three or more hyphens, asterisks,
        /// or underscores on a line by themselves. If you wish, you may use
        /// spaces between the hyphens or asterisks."
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="x">Horizontal rule character.</param>
        /// <returns><c>true</c> if successfully matched.</returns>
        /// <remarks>Original: int scan_hrule(string s, int pos, int sourceLength)</remarks>
        public static bool ScanHorizontalRule(BlockParserInfo info, char x)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            // @"^([\*][ ]*){3,}[\s]*$",
            // @"^([_][ ]*){3,}[\s]*$",
            // @"^([-][ ]*){3,}[\s]*$",

            var count = 0;
            var ipos = pos;
            while (ipos < sourceLength)
            {
                var c = s[ipos++];

                if (c == ' ' || c == '\n')
                    continue;

                if (c == x)
                    count++;
                else
                    return false;
            }

            return count >= 3;
        }
    }
}
