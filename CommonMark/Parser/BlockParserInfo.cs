using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Stage 1 block parser state.
    /// </summary>
    public class BlockParserInfo
    {
        private const int CODE_INDENT = 4;
        private const int TabSize = 4;

        internal BlockParserInfo(Block curptr, LineInfo line)
        {
            // container starts at the document root.
            this.Container = curptr.Top;
            this.ParentContainer = this.CurrentContainer = curptr;
            this.LineInfo = line;
            this.IsAllMatched = true;
        }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        public Block Container { get; set; }

        /// <summary>
        /// Gets or sets the parent container.
        /// </summary>
        public Block ParentContainer { get; set; }

        /// <summary>
        /// Gets or sets the current container.
        /// </summary>
        public Block CurrentContainer { get; set; }

        /// <summary>
        /// Gets or sets the last matched container.
        /// </summary>
        public Block LastMatchedContainer { get; set; }

        /// <summary>
        /// Gets the line information.
        /// </summary>
        public LineInfo LineInfo { get; }

        /// <summary>
        /// Gets or sets the character position in the line.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the virtual position in the line that takes TAB expansion into account.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets the position of the first non-space char.
        /// </summary>
        public int FirstNonspace
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the virtual position of the first non-space char, that includes TAB expansion.
        /// </summary>
        public int FirstNonspaceColumn
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the first non-space character.
        /// </summary>
        public char CurrentCharacter { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether all containers have been matched.
        /// </summary>
        public bool IsAllMatched { get; set; }

        /// <summary>
        /// Gets the line string.
        /// </summary>
        public string Line
        {
            get { return LineInfo.Line; }
        }

        /// <summary>
        /// Gets the line indent size.
        /// </summary>
        public int Indent
        {
            get { return FirstNonspaceColumn - Column; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the line is indented.
        /// </summary>
        public bool IsIndented
        {
            get { return Indent >= CODE_INDENT; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether whether the line is blank.
        /// </summary>
        public bool IsBlank
        {
            get { return CurrentCharacter == '\n'; }
        }

        /// <summary>
        /// Updates the current object with first non-space char data.
        /// </summary>
        public void FindFirstNonspace()
        {
            var chars_to_tab = TabSize - (Column % TabSize);
            FirstNonspace = Offset;
            FirstNonspaceColumn = Column;
            while ((CurrentCharacter = Line[FirstNonspace]) != '\n')
            {
                if (CurrentCharacter == ' ')
                {
                    FirstNonspace++;
                    FirstNonspaceColumn++;
                    chars_to_tab--;
                    if (chars_to_tab == 0)
                        chars_to_tab = TabSize;
                }
                else if (CurrentCharacter == '\t')
                {
                    FirstNonspace++;
                    FirstNonspaceColumn += chars_to_tab;
                    chars_to_tab = TabSize;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Advances offset and column by the specified character count.
        /// </summary>
        /// <param name="count">Character count.</param>
        /// <param name="columns"><c>true</c> if TAB expansion is to be taken into account.</param>
        public void AdvanceOffset(int count, bool columns)
        {
            char c;
            while (count > 0 && (c = Line[Offset]) != '\n')
            {
                if (c == '\t')
                {
                    var chars_to_tab = 4 - (Column % TabSize);
                    Column += chars_to_tab;
                    Offset += 1;
                    count -= columns ? chars_to_tab : 1;
                }
                else
                {
                    Offset += 1;
                    Column += 1; // assume ascii; block starts are ascii  
                    count -= 1;
                }
            }
        }

        /// <summary>
        /// Advances offset and column in an indented code line.
        /// </summary>
        public void AdvanceIndentedOffset()
        {
            AdvanceOffset(CODE_INDENT, true);
        }
    }
}
