using CommonMark.Syntax;
using System.Collections.Generic;
using System.Globalization;

namespace CommonMark.Parser
{
    /// <summary>
    /// Base Stage 1 block parser class.
    /// </summary>
    public abstract class BlockParser : IBlockParser
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected BlockParser(CommonMarkSettings settings)
        {
            this.Settings = settings;
        }

        #endregion Constructor

        #region IBlockParser Members

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public abstract char[] Characters
        {
            get;
        }

        /// <summary>
        /// Gets the value indicating whether a handled element is a code block.
        /// </summary>
        /// <value><c>true</c> if a handled element is a code block.</value>
        public virtual bool IsCodeBlock
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the value indicating whether a handled element accepts new lines.
        /// </summary>
        /// <value><c>true</c> if new lines can be added to a handled element.</value>
        public virtual bool IsAcceptsLines
        {
            get { return false; }
        }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if blank lines at the end of the handled element should be discarded.</returns>
        public virtual bool IsDiscardLastBlank(BlockParserInfo info)
        {
            return false;
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public abstract bool Advance(ref BlockParserInfo info);

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public abstract bool Open(ref BlockParserInfo info);

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public abstract bool Close(BlockParserInfo info);

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

            block.InlineContent = InlineMethods.parse_inlines(block, subj, subj.ReferenceMap, parameters);
            block.StringContent = null;

            if (sc.PositionTracker != null)
            {
                sc.PositionTracker.AddBlockOffset(-delta);
                AdjustInlineSourcePosition(block.InlineContent, sc.PositionTracker, ref inlineStack);
            }

            return true;
        }

        /// <summary>
        /// Determines whether elements of the specified kind can be contained one in another.
        /// </summary>
        /// <param name="parentTag">Block element tag.</param>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if elements having <paramref name="parentTag"/> can contain elements having <paramref name="childTag"/>.</returns>
        protected virtual bool CanContain(BlockTag parentTag, BlockTag childTag)
        {
            return (parentTag == BlockTag.Document ||
                     parentTag == BlockTag.BlockQuote ||
                     parentTag == BlockTag.ListItem ||
                     (parentTag == BlockTag.List && childTag == BlockTag.ListItem));
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

        #region InitializeParsers

        internal static IBlockParser[] InitializeParsers(CommonMarkSettings settings)
        {
            var parsers = new IBlockParser[(int)BlockTag.Count];

            parsers[(int)BlockTag.BlockQuote] = new Blocks.BlockQuoteParser(settings);
            parsers[(int)BlockTag.List] = new Blocks.ListParser(settings);
            parsers[(int)BlockTag.ListItem] = new Blocks.ListItemParser(settings);
            parsers[(int)BlockTag.IndentedCode] = new Blocks.IndentedCodeParser(settings);
            parsers[(int)BlockTag.AtxHeader] = new Blocks.AtxHeaderParser(settings);
            parsers[(int)BlockTag.SETextHeader] = new Blocks.SETextHeaderParser(settings);
            parsers[(int)BlockTag.FencedCode] = new Blocks.FencedCodeParser(settings);
            parsers[(int)BlockTag.HtmlBlock] = new Blocks.HtmlBlockParser(settings);
            parsers[(int)BlockTag.Paragraph] = new Blocks.ParagraphParser(settings);
            parsers[(int)BlockTag.HorizontalRuler] = new Blocks.HorizontalRulerParser(settings);

            return parsers;
        }

        #endregion InitializeParsers
    }
}
