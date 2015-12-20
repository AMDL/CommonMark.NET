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
        protected BlockParser(CommonMarkSettings settings, BlockTag tag)
        {
            this.Settings = settings;
            this.Tag = tag;
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
        /// Gets the block element delimiter handlers.
        /// </summary>
        public virtual IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                return null;
            }
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
        protected CommonMarkSettings Settings
        {
            get;
        }

        /// <summary>
        /// Determines whether the content consists of a single line.
        /// </summary>
        /// <param name="content">String content.</param>
        /// <returns><c>true</c> if the content consists of a single line.</returns>
        public static bool ContainsSingleLine(StringContent content)
        {
            if (content == null)
                return true;
            var i = content.IndexOf('\n');
            return (i == -1 || i == content.Length - 1);
        }

        /// <summary>
        /// Adds a new line to the block element.
        /// </summary>
        public static void AddLine(Block block, LineInfo lineInfo, string ln, int offset, int length = -1)
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
        public static Block CreateChildBlock(BlockParserInfo info, BlockTag blockType, int startColumn, CommonMarkSettings settings)
        {
            var parent = info.Container;
            var line = info.LineInfo;

            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!settings.BlockParserParameters.CanContain(parent.Tag, blockType))
            {
                BlockMethods.Finalize(parent, line, settings);
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

        internal static IEnumerable<IBlockParser> InitializeParsers(CommonMarkSettings settings)
        {
            yield return new Blocks.DocumentParser(settings);
            yield return new Blocks.BlockQuoteParser(settings);
            yield return new Blocks.BulletListParser(settings);
            yield return new Blocks.OrderedListParser(settings);
            yield return new Blocks.ListItemParser(settings);
            yield return new Blocks.IndentedCodeParser(settings);
            yield return new Blocks.AtxHeaderParser(settings);
            yield return new Blocks.SETextHeaderParser(settings);
            yield return new Blocks.FencedCodeParser(settings);
            yield return new Blocks.HtmlBlockParser(settings);
            yield return new Blocks.ParagraphParser(settings);
            yield return new Blocks.HorizontalRulerParser(settings);
        }

        #endregion InitializeParsers
    }
}
