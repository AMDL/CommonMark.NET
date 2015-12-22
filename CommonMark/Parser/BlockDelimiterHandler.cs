using CommonMark.Syntax;

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
        /// <param name="tag">Block element tag.</param>
        /// <param name="characters">Handled characters.</param>
        protected BlockDelimiterHandler(CommonMarkSettings settings, BlockTag tag, params char[] characters)
        {
            this.Settings = settings;
            this.Characters = characters;
            this.Tag = tag;
        }

        /// <summary>
        /// Gets the characters that are handled by this parser.
        /// </summary>
        /// <value>The characters that can open a handled element.</value>
        public char[] Characters
        {
            get;
        }

        /// <summary>
        /// Gets the element tag.
        /// </summary>
        /// <value>The tag of the elements opened by this parser.</value>
        public BlockTag Tag
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
        /// Adds a new block as the last child of another.
        /// </summary>
        /// <returns>The child block.</returns>
        /// <remarks>Original: add_child</remarks>
        public Block AppendChildBlock(BlockParserInfo info, BlockTag blockType, int startColumn)
        {
            var parent = info.Container;
            var line = info.LineInfo;

            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!Settings.BlockParserParameters.CanContain(parent.Tag, blockType))
            {
                BlockMethods.Finalize(parent, line, Settings);
                parent = parent.Parent;
            }

            var startPosition = line.IsTrackingPositions ? line.CalculateOrigin(startColumn, true) : line.LineOffset;
#pragma warning disable 0618
            Block child = new Block(blockType, line.LineNumber, startColumn + 1, startPosition);
#pragma warning restore 0618
            child.Parent = parent;
            child.Top = parent.Top;

            var lastChild = parent.LastChild;
            if (lastChild != null)
            {
                lastChild.NextSibling = child;
#pragma warning disable 0618
                child.Previous = lastChild;
#pragma warning restore 0618
            }
            else
            {
                parent.FirstChild = child;
            }

            parent.LastChild = child;
            return child;
        }

        /// <summary>
        /// Scans a horizontal rule line: "...three or more hyphens, asterisks,
        /// or underscores on a line by themselves. If you wish, you may use
        /// spaces between the hyphens or asterisks."
        /// Assumes that there is a <paramref name="character"/> at the current position.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="character">Horizontal rule character.</param>
        /// <param name="minCount">Minimum horizontal rule character count.</param>
        /// <returns><c>true</c> if successfully matched.</returns>
        /// <remarks>Original: int scan_hrule(string s, int pos, int sourceLength)</remarks>
        public static bool ScanHorizontalRule(BlockParserInfo info, char character, int minCount = 3)
        {
            var line = info.Line;
            var offset = info.FirstNonspace;
            var length = line.Length;

            // @"^([\*][ ]*){3,}[\s]*$",
            // @"^([_][ ]*){3,}[\s]*$",
            // @"^([-][ ]*){3,}[\s]*$",

            if (offset >= length - minCount)
                return false;

            var count = 1;
            while (++offset < length)
            {
                var curChar = line[offset];

                if (curChar == ' ' || curChar == '\n')
                    continue;

                if (curChar == character)
                    count++;
                else
                    return false;
            }

            return count >= minCount;
        }
    }
}
