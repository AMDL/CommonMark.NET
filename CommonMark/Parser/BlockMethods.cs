using System.Collections.Generic;
using System.Globalization;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal static class BlockMethods
    {
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool CanContain(BlockTag parent_type, BlockTag child_type)
        {
            return (parent_type == BlockTag.Document ||
                     parent_type == BlockTag.BlockQuote ||
                     parent_type == BlockTag.ListItem ||
                     (parent_type == BlockTag.List && child_type == BlockTag.ListItem));
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool AcceptsLines(BlockTag block_type)
        {
            return (block_type == BlockTag.Paragraph ||
                    block_type == BlockTag.AtxHeader ||
                    block_type == BlockTag.IndentedCode ||
                    block_type == BlockTag.FencedCode);
        }

        private static void AddLine(Block block, LineInfo lineInfo, string ln, int offset, int length = -1)
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
        /// Check to see if a block ends with a blank line, descending if needed into lists and sublists.
        /// </summary>
        private static bool EndsWithBlankLine(Block block)
        {
            while (true)
            {
                if (block.IsLastLineBlank)
                    return true;

                if (block.Tag != BlockTag.List && block.Tag != BlockTag.ListItem)
                    return false;

                block = block.LastChild;

                if (block == null)
                    return false;
            }
        }

        /// <summary>
        /// Break out of all containing lists
        /// </summary>
        private static void BreakOutOfLists(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            Block container = info.Container;
            Block b = container.Top;

            // find first containing list:
            while (b != null && b.Tag != BlockTag.List)
                b = b.LastChild;

            if (b != null)
            {
                while (container != null && container != b)
                {
                    Finalize(container, info.LineInfo, settings);
                    container = container.Parent;
                }

                Finalize(b, info.LineInfo, settings);
                info.Container = b.Parent;
            }
        }

        public static void Finalize(Block b, LineInfo line, CommonMarkSettings settings)
        {
            // don't do anything if the block is already closed
            if (!b.IsOpen)
                return;

            b.IsOpen = false;

            if (line.IsTrackingPositions)
            {
                // (b.SourcePosition >= line.LineOffset) determines if the block started on this line.
                if (b.SourcePosition >= line.LineOffset && line.Line != null)
                    b.SourceLastPosition = line.CalculateOrigin(line.Line.Length, false);
                else
                    b.SourceLastPosition = line.CalculateOrigin(0, false);
            }

#pragma warning disable 0618
            b.EndLine = (line.LineNumber > b.StartLine) ? line.LineNumber - 1 : line.LineNumber;
#pragma warning restore 0618

            switch (b.Tag)
            {
                case BlockTag.Paragraph:
                    var sc = b.StringContent;
                    if (!sc.StartsWith('['))
                        break;

                    var subj = new Subject(b.Top.ReferenceMap);
                    sc.FillSubject(subj);
                    var origPos = subj.Position;
                    while (subj.Position < subj.Buffer.Length
                        && subj.Buffer[subj.Position] == '['
                        && 0 != InlineMethods.ParseReference(subj, settings.InlineParserParameters))
                    {
                    }

                    if (subj.Position != origPos)
                    {
                        sc.Replace(subj.Buffer, subj.Position, subj.Buffer.Length - subj.Position);

                        if (sc.PositionTracker != null)
                            sc.PositionTracker.AddBlockOffset(subj.Position - origPos);

                        if (Utilities.IsFirstLineBlank(subj.Buffer, subj.Position))
                            b.Tag = BlockTag.ReferenceDefinition;
                    }

                    break;

                case BlockTag.IndentedCode:
                    b.StringContent.RemoveTrailingBlankLines();
                    break;

                case BlockTag.FencedCode:
                    // first line of contents becomes info
                    var firstlinelen = b.StringContent.IndexOf('\n') + 1;
                    b.FencedCodeData.Info = InlineMethods.Unescape(b.StringContent.TakeFromStart(firstlinelen, true).Trim());
                    break;

                case BlockTag.List: // determine tight/loose status
                    b.ListData.IsTight = true; // tight by default
                    var item = b.FirstChild;
                    Block subitem;

                    while (item != null)
                    {
                        // check for non-final non-empty list item ending with blank line:
                        if (item.IsLastLineBlank && item.NextSibling != null)
                        {
                            b.ListData.IsTight = false;
                            break;
                        }

                        // recurse into children of list item, to see if there are spaces between them:
                        subitem = item.FirstChild;
                        while (subitem != null)
                        {
                            if (EndsWithBlankLine(subitem) && (item.NextSibling != null || subitem.NextSibling != null))
                            {
                                b.ListData.IsTight = false;
                                break;
                            }

                            subitem = subitem.NextSibling;
                        }

                        if (!b.ListData.IsTight)
                            break;

                        item = item.NextSibling;
                    }

                    break;
            }
        }

        /// <summary>
        /// Adds a new block as child of another. Return the child.
        /// </summary>
        /// <remarks>Original: add_child</remarks>
        public static Block CreateChildBlock(Block parent, LineInfo line, BlockTag blockType, int startColumn, CommonMarkSettings settings)
        {
            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!CanContain(parent.Tag, blockType))
            {
                Finalize(parent, line, settings);
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

        #region Process

        /// <summary>
        /// Walk through the block, its children and siblings, parsing string content into inline content where appropriate.
        /// </summary>
        /// <param name="block">The document level block from which to start the processing.</param>
        /// <param name="refmap">The reference mapping used when parsing links.</param>
        /// <param name="settings">The settings that influence how the inline parsing is performed.</param>
        public static void ProcessInlines(Block block, Dictionary<string, Reference> refmap, CommonMarkSettings settings)
        {
            Stack<Inline> inlineStack = null;
            var stack = new Stack<Block>();
            var subj = new Subject(refmap);

            var processors = settings.BlockParserParameters.Processors;

            while (block != null)
            {
                var processor = processors[(int)block.Tag];
                if (processor != null)
                {
                    processor(block, subj, refmap, ref inlineStack, settings);
                }

                if (block.FirstChild != null)
                {
                    if (block.NextSibling != null)
                        stack.Push(block.NextSibling);

                    block = block.FirstChild;
                }
                else if (block.NextSibling != null)
                {
                    block = block.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    block = stack.Pop();
                }
                else
                {
                    block = null;
                }
            }
        }

        private static bool ProcessInlines(Block block, Subject subj, Dictionary<string, Reference> refmap, ref Stack<Inline> inlineStack, CommonMarkSettings settings)
        {
            return ProcessInlines(block, subj, refmap, ref inlineStack, settings.InlineParserParameters);
        }

        public static bool ProcessInlines(Block block, Subject subj, Dictionary<string, Reference> refmap, ref Stack<Inline> inlineStack, InlineParserParameters parameters)
        {
            var sc = block.StringContent;
            if (sc == null)
                return false;

            sc.FillSubject(subj);
            var delta = subj.Position;

            block.InlineContent = InlineMethods.parse_inlines(block, subj, refmap, parameters);
            block.StringContent = null;

            if (sc.PositionTracker != null)
            {
                sc.PositionTracker.AddBlockOffset(-delta);
                AdjustInlineSourcePosition(block.InlineContent, sc.PositionTracker, ref inlineStack);
            }

            return true;
        }

        public static BlockProcessorDelegate[] InitializeProcessors()
        {
            var p = new BlockProcessorDelegate[(int)BlockTag.Count];
            p[(int)BlockTag.Paragraph] = p[(int)BlockTag.AtxHeader] = p[(int)BlockTag.SETextHeader] = ProcessInlines;
            return p;
        }

        #endregion Process

        /// <summary>
        /// Attempts to parse a list item marker (bullet or enumerated).
        /// On success, returns length of the marker, and populates
        /// data with the details.  On failure, returns 0.
        /// </summary>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        private static int ParseListMarker(string ln, int pos, out ListData data)
        {
            char c;
            int startpos;
            data = null;
            var len = ln.Length;

            startpos = pos;
            c = ln[pos];

            if (c == '+' || c == '•' || ((c == '*' || c == '-') && 0 == Scanner.scan_hrule(ln, pos, len)))
            {
                pos++;
                if (pos == len || (ln[pos] != ' ' && ln[pos] != '\n'))
                    return 0;

                data = new ListData();
                data.BulletChar = c;
                data.Start = 1;
            }
            else if (c >= '0' && c <= '9')
            {

                int start = c - '0';

                while (pos < len - 1)
                {
                    c = ln[++pos];
                    // We limit to 9 digits to avoid overflow, This also seems to be the limit for 'start' in some browsers. 
                    if (c >= '0' && c <= '9' && start < 100000000)
                        start = start * 10 + (c - '0');
                    else
                        break;
                }

                if (pos >= len - 1 || (c != '.' && c != ')'))
                    return 0;

                pos++;
                if (pos == len || (ln[pos] != ' ' && ln[pos] != '\n'))
                    return 0;

                data = new ListData();
                data.ListType = ListType.Ordered;
                data.BulletChar = '\0';
                data.Start = start;
                data.Delimiter = (c == '.' ? ListDelimiter.Period : ListDelimiter.Parenthesis);

            }
            else
            {
                return 0;
            }

            return (pos - startpos);
        }

        internal static bool ContainsSingleLine(StringContent content)
        {
            if (content == null)
                return true;
            var i = content.IndexOf('\n');
            return (i == -1 || i == content.Length - 1);
        }

        private static bool ListsMatch(ListData listData, ListData itemData)
        {
            return (listData.ListType == itemData.ListType &&
                    listData.Delimiter == itemData.Delimiter &&
                // list_data.marker_offset == item_data.marker_offset &&
                    listData.BulletChar == itemData.BulletChar);
        }

        // Process one line at a time, modifying a block.
        // Returns 0 if successful.  curptr is changed to point to
        // the currently open block.
        public static void IncorporateLine(LineInfo line, ref Block curptr, CommonMarkSettings settings)
        {
            var info = new BlockParserInfo(curptr, line);

            Advance(ref info, settings);

            Initialize(ref info, settings);

            Finalize(info, settings);

            curptr = info.ParentContainer;
        }

        #region Advance

        private static void Advance(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            var parameters = settings.BlockParserParameters;

            // for each containing block, try to parse the associated line start.
            // bail out on failure:  container will point to the last matching block.

            while (info.Container.LastChild != null && info.Container.LastChild.IsOpen)
            {
                info.Container = info.Container.LastChild;

                info.FindFirstNonspace();

                var advancer = parameters.Advancers[(int)info.Container.Tag];
                if (advancer != null)
                {
                    // The advancer will do the job
                    info.IsAllMatched &= advancer(ref info);
                }

                if (!info.IsAllMatched)
                {
                    info.Container = info.Container.Parent;  // back up to last matching block
                    break;
                }
            }

            info.LastMatchedContainer = info.Container;

            // check to see if we've hit 2nd blank line, break out of list:
            if (info.IsBlank && info.Container.IsLastLineBlank)
                BreakOutOfLists(ref info, settings);
        }

        public static BlockParserDelegate[] InitializeAdvancers()
        {
            var p = new BlockParserDelegate[(int)BlockTag.Count];

            p[(int)BlockTag.BlockQuote] = AdvanceBlockQuote;
            p[(int)BlockTag.ListItem] = AdvanceListItem;
            p[(int)BlockTag.IndentedCode] = AdvanceIndentedCode;
            p[(int)BlockTag.AtxHeader] = AdvanceHeader;
            p[(int)BlockTag.SETextHeader] = AdvanceHeader;
            p[(int)BlockTag.FencedCode] = AdvanceFencedCode;
            p[(int)BlockTag.HtmlBlock] = AdvanceHtmlBlock;
            p[(int)BlockTag.Paragraph] = AdvanceParagraph;

            return p;
        }

        private static bool AdvanceBlockQuote(ref BlockParserInfo info)
        {
            if (!info.IsIndented && info.CurrentCharacter == '>')
            {
                info.AdvanceOffset(info.Indent + 1, true);
                if (info.Line[info.Offset] == ' ')
                    info.Offset++;
                return true;
            }
            return false;
        }

        private static bool AdvanceListItem(ref BlockParserInfo info)
        {
            if (info.Indent >= info.Container.ListData.MarkerOffset + info.Container.ListData.Padding)
            {
                info.AdvanceOffset(info.Container.ListData.MarkerOffset + info.Container.ListData.Padding, true);
                return true;
            }
            if (info.IsBlank && info.Container.FirstChild != null)
            {
                // if container->first_child is NULL, then the opening line
                // of the list item was blank after the list marker; in this
                // case, we are done with the list item.
                info.AdvanceOffset(info.FirstNonspace - info.Offset, false);
                return true;
            }
            return false;
        }

        private static bool AdvanceIndentedCode(ref BlockParserInfo info)
        {
            if (info.IsIndented)
            {
                info.AdvanceIndentedOffset();
                return true;
            }
            if (info.IsBlank)
            {
                info.AdvanceOffset(info.FirstNonspace - info.Offset, false);
                return true;
            }
            return false;
        }

        private static bool AdvanceHeader(ref BlockParserInfo info)
        {
            // a header can never contain more than one line
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
            }
            return false;
        }

        public static bool AdvanceFencedCode(ref BlockParserInfo info)
        {
            // -1 means we've seen closer 
            if (info.Container.FencedCodeData.FenceLength == -1)
            {
                if (info.IsBlank)
                    info.Container.IsLastLineBlank = true;
                return false;
            }

            // skip optional spaces of fence offset
            var i = info.Container.FencedCodeData.FenceOffset;
            while (i > 0 && info.Line[info.Offset] == ' ')
            {
                info.Offset++;
                info.Column++;
                i--;
            }

            return true;
        }

        private static bool AdvanceHtmlBlock(ref BlockParserInfo info)
        {
            // all other block types can accept blanks
            if (info.IsBlank && info.Container.HtmlBlockType >= HtmlBlockType.InterruptingBlock)
            {
                info.Container.IsLastLineBlank = true;
                return false;
            }
            return true;
        }

        private static bool AdvanceParagraph(ref BlockParserInfo info)
        {
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
                return false;
            }
            return true;
        }

        #endregion Advance

        #region Initialize

        private static void Initialize(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            var initializers = settings.BlockParserParameters.Initializers;

            var maybeLazy = info.CurrentContainer.Tag == BlockTag.Paragraph;

            // unless last matched container is code block, try new container starts:
            while (info.Container.Tag != BlockTag.FencedCode &&
                   info.Container.Tag != BlockTag.IndentedCode &&
                   info.Container.Tag != BlockTag.HtmlBlock)
            {
                info.FindFirstNonspace();

                BlockParserDelegate initializer;
                if (info.CurrentCharacter < initializers.Length && (initializer = initializers[info.CurrentCharacter]) != null && initializer(ref info))
                {
                    // The initializer will do the job
                }
                else if (info.IsIndented && !maybeLazy && !info.IsBlank)
                {
                    info.AdvanceIndentedOffset();
                    info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.IndentedCode, info.Offset, settings);
                }
                else
                {
                    break;
                }

                if (AcceptsLines(info.Container.Tag))
                {
                    // if it's a line container, it can't contain other containers
                    break;
                }

                maybeLazy = false;
            }
        }

        internal static BlockParserDelegate[] InitializeInitializers(CommonMarkSettings settings)
        {
            var p = new BlockParserDelegate['•' + 1];

            BlockParserDelegate initList = (ref BlockParserInfo info) => InitalizeList(info, info.Indent, settings);

            for (var c = '0'; c <= '9'; c++)
                p[c] = initList;

            p['+'] = initList;
            p['•'] = initList;

            p['*'] = DelegateBlockParser.Merge(
                (ref BlockParserInfo info) => InitializeHorizontalRule(ref info, settings),
                initList);
            p['-'] = DelegateBlockParser.Merge(
                (ref BlockParserInfo info) => InitializeSETextHeader(ref info, settings),
                (ref BlockParserInfo info) => InitializeHorizontalRule(ref info, settings),
                initList);

            p['#'] = (ref BlockParserInfo info) => InitializeAtxHeader(ref info, settings);
            p['<'] = (ref BlockParserInfo info) => InitializeHtmlBlock(ref info, settings);
            p['='] = (ref BlockParserInfo info) => InitializeSETextHeader(ref info, settings);
            p['>'] = (ref BlockParserInfo info) => InitializeBlockQuote(ref info, settings);
            p['_'] = (ref BlockParserInfo info) => InitializeHorizontalRule(ref info, settings);
            p['`'] = (ref BlockParserInfo info) => InitializeFencedCode(ref info, settings);
            p['~'] = (ref BlockParserInfo info) => InitializeFencedCode(ref info, settings);

            return p;
        }

        private static bool InitializeBlockQuote(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            if (info.IsIndented)
                return false;

            info.AdvanceOffset(info.FirstNonspace + 1 - info.Offset, false);

            // optional following character
            if (info.Line[info.Offset] == ' ')
            {
                info.Offset++;
                info.Column++;
            }
            info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.BlockQuote, info.FirstNonspace, settings);
            return true;
        }

        private static bool InitializeAtxHeader(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            int matched;
            int i;
            if (!info.IsIndented && 0 != (matched = Scanner.scan_atx_header_start(info.Line, info.FirstNonspace, info.Line.Length, out i)))
            {
                info.AdvanceOffset(info.FirstNonspace + matched - info.Offset, false);
                info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.AtxHeader, info.FirstNonspace, settings);
                info.Container.HeaderLevel = i;
                return true;
            }
            return false;
        }

        private static bool InitializeFencedCode(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            return InitializeFencedCode(ref info, settings, settings.BlockParserParameters);
        }

        public static bool InitializeFencedCode(ref BlockParserInfo info, CommonMarkSettings settings, BlockParserParameters parameters)
        {
            int matched;
            if (!info.IsIndented && 0 != (matched = Scanner.scan_open_code_fence(info.Line, info.FirstNonspace, info.Line.Length, parameters)))
            {
                info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.FencedCode, info.FirstNonspace, settings);
                info.Container.FencedCodeData = new FencedCodeData
                {
                    FenceChar = info.CurrentCharacter,
                    FenceLength = matched,
                    FenceOffset = info.FirstNonspace - info.Offset,
                };
                info.AdvanceOffset(info.FirstNonspace + matched - info.Offset, false);
                return true;
            }
            return false;
        }

        private static bool InitializeHtmlBlock(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            int matched;
            if (!info.IsIndented && (0 != (matched = (int)Scanner.scan_html_block_start(info.Line, info.FirstNonspace, info.Line.Length))
                || (info.Container.Tag != BlockTag.Paragraph && 0 != (matched = (int)Scanner.scan_html_block_start_7(info.Line, info.FirstNonspace, info.Line.Length)))))
            {
                info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.HtmlBlock, info.FirstNonspace, settings);
                info.Container.HtmlBlockType = (HtmlBlockType)matched;
                // note, we don't adjust offset because the tag is part of the text
                return true;
            }
            return false;
        }

        private static bool InitializeSETextHeader(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            int matched;
            if (!info.IsIndented && info.Container.Tag == BlockTag.Paragraph && 0 != (matched = Scanner.scan_setext_header_line(info.Line, info.FirstNonspace, info.Line.Length))
                && ContainsSingleLine(info.Container.StringContent))
            {
                info.Container.Tag = BlockTag.SETextHeader;
                info.Container.HeaderLevel = matched;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }

        private static bool InitializeHorizontalRule(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            if (!info.IsIndented && !(info.Container.Tag == BlockTag.Paragraph && !info.IsAllMatched) && 0 != (Scanner.scan_hrule(info.Line, info.FirstNonspace, info.Line.Length)))
            {
                // it's only now that we know the line is not part of a setext header:
                info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.HorizontalRuler, info.FirstNonspace, settings);
                Finalize(info.Container, info.LineInfo, settings);
                info.Container = info.Container.Parent;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }

        private static bool InitalizeList(BlockParserInfo info, int indent, CommonMarkSettings settings)
        {
            int matched = 0;
            ListData data;
            if ((!info.IsIndented || info.Container.Tag == BlockTag.List) && 0 != (matched = ParseListMarker(info.Line, info.FirstNonspace, out data)))
            {
                // compute padding:
                info.AdvanceOffset(info.FirstNonspace + matched - info.Offset, false);
                var i = 0;
                while (i <= 5 && info.Line[info.Offset + i] == ' ')
                    i++;

                // i = number of spaces after marker, up to 5
                if (i >= 5 || i < 1 || info.Line[info.Offset] == '\n')
                {
                    data.Padding = matched + 1;
                    if (i > 0)
                    {
                        info.Column++;
                        info.Offset++;
                    }
                }
                else
                {
                    data.Padding = matched + i;
                    info.AdvanceOffset(i, true);
                }

                // check container; if it's a list, see if this list item
                // can continue the list; otherwise, create a list container.

                data.MarkerOffset = indent;

                if (info.Container.Tag != BlockTag.List || !ListsMatch(info.Container.ListData, data))
                {
                    info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.List, info.FirstNonspace, settings);
                    info.Container.ListData = data;
                }

                // add the list item
                info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.ListItem, info.FirstNonspace, settings);
                info.Container.ListData = data;
            }

            return matched > 0;
        }

        #endregion Initialize

        #region Finalize

        private static void Finalize(BlockParserInfo info, CommonMarkSettings settings)
        {
            var parameters = settings.BlockParserParameters;

            info.FindFirstNonspace();

            if (info.IsBlank && info.Container.LastChild != null)
            {
                info.Container.LastChild.IsLastLineBlank = true;
            }

            // block quote lines are never blank as they start with >
            // and we don't count blanks in fenced code for purposes of tight/loose
            // lists or breaking out of lists.  we also don't set last_line_blank
            // on an empty list item.
            info.Container.IsLastLineBlank = (info.IsBlank &&
                                          info.Container.Tag != BlockTag.BlockQuote &&
                                          info.Container.Tag != BlockTag.SETextHeader &&
                                          info.Container.Tag != BlockTag.FencedCode &&
                                          !(info.Container.Tag == BlockTag.ListItem &&
                                            info.Container.FirstChild == null &&
                                            info.Container.SourcePosition >= info.LineInfo.LineOffset));

            Block cont = info.Container;
            while (cont.Parent != null)
            {
                cont.Parent.IsLastLineBlank = false;
                cont = cont.Parent;
            }

            Block cur = info.CurrentContainer;
            if (cur != info.LastMatchedContainer &&
                info.Container == info.LastMatchedContainer &&
                !info.IsBlank &&
                cur.Tag == BlockTag.Paragraph &&
                cur.StringContent.Length > 0)
            {
                AddLine(cur, info.LineInfo, info.Line, info.Offset);
            }
            else
            { // not a lazy continuation

                // finalize any blocks that were not matched and set cur to container:
                while (cur != info.LastMatchedContainer)
                {
                    Finalize(cur, info.LineInfo, settings);
                    cur = cur.Parent;

                    if (cur == null)
                        throw new CommonMarkException("Cannot finalize container block. Last matched container tag = " + info.LastMatchedContainer.Tag);
                }

                BlockParserDelegate finalizer;
                if ((finalizer = parameters.Finalizers[(int)info.Container.Tag]) != null && finalizer(ref info))
                {
                    // The finalizer will do the job
                }
                else if (info.IsBlank)
                {
                    // ??? do nothing
                }
                else
                {
                    // create paragraph container for line
                    info.Container = CreateChildBlock(info.Container, info.LineInfo, BlockTag.Paragraph, info.FirstNonspace, settings);
                    AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace);
                }

                info.ParentContainer = info.Container;
            }
        }

        public static BlockParserDelegate[] InitializeFinalizers(CommonMarkSettings settings)
        {
            var p = new BlockParserDelegate[(int)BlockTag.Count];

            p[(int)BlockTag.IndentedCode] = (ref BlockParserInfo info) => FinalizeIndentedCode(ref info, settings);
            p[(int)BlockTag.FencedCode] = (ref BlockParserInfo info) => FinalizeFencedCode(ref info, settings);
            p[(int)BlockTag.HtmlBlock] = (ref BlockParserInfo info) => FinalizeHtmlBlock(ref info, settings);
            p[(int)BlockTag.AtxHeader] = (ref BlockParserInfo info) => FinalizeAtxHeader(ref info, settings);
            p[(int)BlockTag.SETextHeader] = (ref BlockParserInfo info) => NoOpFinalize(ref info, settings);
            p[(int)BlockTag.HorizontalRuler] = (ref BlockParserInfo info) => NoOpFinalize(ref info, settings);
            p[(int)BlockTag.Paragraph] = (ref BlockParserInfo info) => AddLineFinalize(ref info, settings);

            return p;
        }

        private static bool FinalizeIndentedCode(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            AddLine(info.Container, info.LineInfo, info.Line, info.Offset);
            return true;
        }

        private static bool FinalizeFencedCode(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            return FinalizeFencedCode(ref info, settings.BlockParserParameters);
        }

        public static bool FinalizeFencedCode(ref BlockParserInfo info, BlockParserParameters parameters)
        {
            if (!info.IsIndented && info.CurrentCharacter == info.Container.FencedCodeData.FenceChar
              && (0 != Scanner.scan_close_code_fence(info.Line, info.FirstNonspace, info.Container.FencedCodeData.FenceLength, info.Line.Length, parameters)))
            {
                // if closing fence, set fence length to -1. it will be closed when the next line is processed. 
                info.Container.FencedCodeData.FenceLength = -1;
            }
            else
            {
                AddLine(info.Container, info.LineInfo, info.Line, info.Offset);
            }
            return true;
        }

        private static bool FinalizeHtmlBlock(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            AddLine(info.Container, info.LineInfo, info.Line, info.Offset);

            if (Scanner.scan_html_block_end(info.Container.HtmlBlockType, info.Line, info.FirstNonspace, info.Line.Length))
            {
                Finalize(info.Container, info.LineInfo, settings);
                info.Container = info.Container.Parent;
            }

            return true;
        }

        private static bool FinalizeAtxHeader(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            if (info.IsBlank)
            {
                AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace);
                return false;
            }

            int p = info.Line.Length - 1;

            // trim trailing spaces
            while (p >= 0 && (info.Line[p] == ' ' || info.Line[p] == '\n'))
                p--;

            // if string ends in #s, remove these:
            while (p >= 0 && info.Line[p] == '#')
                p--;

            // there must be a space before the last hashtag
            if (p < 0 || info.Line[p] != ' ')
                p = info.Line.Length - 1;

            AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace, p - info.FirstNonspace + 1);
            Finalize(info.Container, info.LineInfo, settings);
            info.Container = info.Container.Parent;
            return true;
        }

        private static bool AddLineFinalize(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            if (info.IsBlank)
                return false;

            AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace);
            return true;
        }

        private static bool NoOpFinalize(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            return true;
        }

        #endregion Finalize
    }
}
