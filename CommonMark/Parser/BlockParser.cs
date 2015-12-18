using CommonMark.Syntax;
using System.Collections.Generic;
using System.Globalization;

namespace CommonMark.Parser
{
    /// <summary>
    /// Base block parser class.
    /// </summary>
    public abstract class BlockParser : ElementParser, IBlockParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="characters">Handled characters.</param>
        protected BlockParser(CommonMarkSettings settings, BlockTag tag, params char[] characters)
        {
            this.Settings = settings;
            this.Tag = tag;
            this.Characters = characters;
        }

        #endregion Constructors

        #region IBlockParser Members

        /// <summary>
        /// Gets the element tag.
        /// </summary>
        /// <value>The element tag handled by this parser.</value>
        public BlockTag Tag
        {
            get;
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public char[] Characters
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value indicating whether a handled element is a list.
        /// </summary>
        public bool IsList
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the value indicating whether a handled element is a code block.
        /// </summary>
        /// <value><c>true</c> if a handled element is a code block.</value>
        public bool IsCodeBlock
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the value indicating whether a handled element accepts new lines.
        /// </summary>
        /// <value><c>true</c> if new lines can be added to a handled element.</value>
        public bool IsAcceptsLines
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the value indicating whether the last blank line of a handled element should be discarded.
        /// </summary>
        /// <value><c>true</c> if a blank line at the end of a handled element should always be discarded.</value>
        public bool IsAlwaysDiscardBlanks
        {
            get;
            protected set;
        }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if a blank line at the end of the handled element should be discarded.</returns>
        public virtual bool IsDiscardLastBlank(BlockParserInfo info)
        {
            return IsAlwaysDiscardBlanks;
        }

        /// <summary>
        /// Determines whether a handled element can contain child elements of the specified kind.
        /// </summary>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if handled elements can contain elements having <paramref name="childTag"/>.</returns>
        public virtual bool CanContain(BlockTag childTag)
        {
            return false;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public virtual bool Initialize(ref BlockParserInfo info)
        {
            return false;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public virtual bool Open(ref BlockParserInfo info)
        {
            return false;
        }

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public virtual bool Close(BlockParserInfo info)
        {
            return false;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public virtual bool Finalize(Block container)
        {
            return false;
        }

        /// <summary>
        /// Processes the inline contents of a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <returns><c>true</c> if successful.</returns>
        public virtual bool Process(Block container, Subject subject, ref Stack<Inline> inlineStack)
        {
            return false;
        }

        #endregion IBlockParser Members

        /// <summary>
        /// Gets the common settings object.
        /// </summary>
        protected CommonMarkSettings Settings { get; }

        /// <summary>
        /// Determines whether the content consists of a single line.
        /// </summary>
        /// <param name="content">String content.</param>
        /// <returns><c>true</c> if the content consists of a single line.</returns>
        protected static bool ContainsSingleLine(StringContent content)
        {
            if (content == null)
                return true;
            var i = content.IndexOf('\n');
            return (i == -1 || i == content.Length - 1);
        }

        /// <summary>
        /// Adds a new line to the block element.
        /// </summary>
        protected static void AddLine(Block block, LineInfo lineInfo, string ln, int offset, int length = -1)
        {
            if (!block.IsOpen)
                throw new CommonMarkException(string.Format(CultureInfo.InvariantCulture, "Attempted to add line '{0}' to closed container ({1}).", ln, block.Tag));

            var len = length == -1 ? ln.Length - offset : length;
            if (len <= 0)
                return;

            var curSC = block.StringContent;
            if (curSC == null)
            {
                block.StringContent = curSC = new StringContent();
                if (lineInfo.IsTrackingPositions)
                    curSC.PositionTracker = new PositionTracker(lineInfo.LineOffset);
            }

            if (lineInfo.IsTrackingPositions)
                curSC.PositionTracker.AddOffset(lineInfo, offset, len);

            curSC.Append(ln, offset, len);
        }

        /// <summary>
        /// Adds a new block as child of another. Return the child.
        /// </summary>
        /// <remarks>Original: add_child</remarks>
        protected Block CreateChildBlock(BlockParserInfo info, BlockTag blockType, int startColumn)
        {
            var parent = info.Container;
            var line = info.LineInfo;

            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!CanContain(parent.Tag, blockType))
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
        /// Processes the inline contents of a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <param name="subj">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected static bool ProcessInlines(Block block, Subject subj, ref Stack<Inline> inlineStack, InlineParserParameters parameters)
        {
            var sc = block.StringContent;
            if (sc == null)
                return false;

            sc.FillSubject(subj);
            var delta = subj.Position;

            block.InlineContent = InlineParser.ParseInlines(block, subj, parameters);
            block.StringContent = null;

            if (sc.PositionTracker != null)
            {
                sc.PositionTracker.AddBlockOffset(-delta);
                AdjustInlineSourcePosition(block.InlineContent, sc.PositionTracker, ref inlineStack);
            }

            return true;
        }

        private bool CanContain(BlockTag parentTag, BlockTag childTag)
        {
            return Settings.BlockParserParameters.CanContain(parentTag, childTag);
        }

        private static void AdjustInlineSourcePosition(Inline inline, PositionTracker tracker, ref Stack<Inline> stack)
        {
            if (stack == null)
                stack = new Stack<Inline>();

            while (inline != null)
            {
                inline.SourcePosition = tracker.CalculateInlineOrigin(inline.SourcePosition, true);
                inline.SourceLastPosition = tracker.CalculateInlineOrigin(inline.SourceLastPosition, false);

                if (inline.FirstChild != null)
                {
                    if (inline.NextSibling != null)
                        stack.Push(inline.NextSibling);

                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    inline = stack.Pop();
                }
                else
                {
                    inline = null;
                }
            }
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
        protected bool ScanHorizontalRule(BlockParserInfo info, char x)
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

        #region InitializeParsers

        internal static IBlockParser[] InitializeParsers(CommonMarkSettings settings)
        {
            var parsers = new IBlockParser[(int)BlockTag.Count];

            parsers[(int)BlockTag.Document] = new Blocks.DocumentParser(settings);
            parsers[(int)BlockTag.BlockQuote] = new Blocks.BlockQuoteParser(settings);
            parsers[(int)BlockTag.BulletList] = new Blocks.ListParser(settings, BlockTag.BulletList, BlockTag.ListItem);
            parsers[(int)BlockTag.OrderedList] = new Blocks.ListParser(settings, BlockTag.OrderedList, BlockTag.ListItem);
            parsers[(int)BlockTag.ListItem] = DelegateBlockParser.Merge(BlockTag.ListItem,
                new Blocks.NonRuleBulletListItemParser(settings),
                new Blocks.RuleBulletListItemParser(settings),
                new Blocks.EuropeanNumeralListItemParser(settings));
            parsers[(int)BlockTag.IndentedCode] = new Blocks.IndentedCodeParser(settings);
            parsers[(int)BlockTag.AtxHeader] = new Blocks.AtxHeaderParser(settings);
            parsers[(int)BlockTag.SETextHeader] = DelegateBlockParser.Merge(BlockTag.SETextHeader,
                new Blocks.SETextHeaderParser(settings, '=', 1),
                new Blocks.SETextHeaderParser(settings, '-', 2));
            parsers[(int)BlockTag.FencedCode] = DelegateBlockParser.Merge(BlockTag.FencedCode,
                new Blocks.FencedCodeParser(settings, BlockTag.FencedCode, '`'),
                new Blocks.FencedCodeParser(settings, BlockTag.FencedCode, '~'));
            parsers[(int)BlockTag.HtmlBlock] = new Blocks.HtmlBlockParser(settings);
            parsers[(int)BlockTag.Paragraph] = new Blocks.ParagraphParser(settings);
            parsers[(int)BlockTag.HorizontalRuler] = DelegateBlockParser.Merge(BlockTag.HorizontalRuler,
                new Blocks.HorizontalRulerParser(settings, '*'),
                new Blocks.HorizontalRulerParser(settings, '-'),
                new Blocks.HorizontalRulerParser(settings, '_'));

            return parsers;
        }

        #endregion InitializeParsers
    }
}
