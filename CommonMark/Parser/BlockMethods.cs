using System.Collections.Generic;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal static class BlockMethods
    {
        #region BreakOutOfLists

        /// <summary>
        /// Break out of all containing lists
        /// </summary>
        private static void BreakOutOfLists(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            Block container = info.Container;
            Block b = container.Top;

            // find first containing list:
            while (b != null && !settings.BlockParserParameters.IsList(b.Tag))
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

        #endregion BreakOutOfLists

        #region Finalize

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

            BlockFinalizerDelegate finalizer;
            if ((finalizer = settings.BlockParserParameters.Finalizers[(int)b.Tag]) != null)
            {
                finalizer(b);
            }
        }

        #endregion

        #region ProcessInlines

        /// <summary>
        /// Walk through the block, its children and siblings, parsing string content into inline content where appropriate.
        /// </summary>
        /// <param name="block">The document level block from which to start the processing.</param>
        /// <param name="document">Document data.</param>
        /// <param name="settings">The settings that influence how the inline parsing is performed.</param>
        public static void ProcessInlines(Block block, DocumentData document, CommonMarkSettings settings)
        {
            Stack<Inline> inlineStack = null;
            var stack = new Stack<Block>();
            var subj = new Subject(document);

            var processors = settings.BlockParserParameters.Processors;
            BlockProcessorDelegate processor;

            while (block != null)
            {
                if ((processor = processors[(int)block.Tag]) != null)
                {
                    processor(block, subj, ref inlineStack);
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

        #endregion ProcessInlines

        #region IncorporateLine

        // Process one line at a time, modifying a block.
        // Returns 0 if successful.  curptr is changed to point to
        // the currently open block.
        public static void IncorporateLine(LineInfo line, ref Block curptr, CommonMarkSettings settings)
        {
            var info = new BlockParserInfo(curptr, line);

            Initialize(ref info, settings);

            Open(ref info, settings);

            Close(info, settings);

            curptr = info.ParentContainer;
        }

        #endregion IncorporateLine

        #region Initialize

        private static void Initialize(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            var initializers = settings.BlockParserParameters.Initializers;
            BlockInitializerDelegate initializer;

            // for each containing block, try to parse the associated line start.
            // bail out on failure:  container will point to the last matching block.

            while (info.Container.LastChild != null && info.Container.LastChild.IsOpen)
            {
                info.Container = info.Container.LastChild;

                info.FindFirstNonspace();

                if ((initializer = initializers[(int)info.Container.Tag]) != null)
                {
                    // The initializer will do the job
                    info.IsAllMatched &= initializer(ref info);
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

        #endregion Initialize

        #region Open

        private static void Open(ref BlockParserInfo info, CommonMarkSettings settings)
        {
            var parameters = settings.BlockParserParameters;
            var openers = parameters.Openers;
            BlockOpenerDelegate opener;

            info.IsMaybeLazy = info.CurrentContainer.Tag == BlockTag.Paragraph;

            // unless last matched container is code block, try new container starts:
            while (!parameters.IsCodeBlock(info.Container.Tag))
            {
                info.FindFirstNonspace();

                if (info.CurrentCharacter < openers.Length && (opener = openers[info.CurrentCharacter]) != null && opener(ref info))
                {
                    // The opener will do the job
                }
                else if (!parameters.OpenIndentedCode(ref info))
                {
                    break;
                }

                if (parameters.IsAcceptsLines(info.Container.Tag))
                {
                    // if it's a line container, it can't contain other containers
                    break;
                }

                info.IsMaybeLazy = false;
            }
        }

        #endregion Open

        #region Close

        private static void Close(BlockParserInfo info, CommonMarkSettings settings)
        {
            var parameters = settings.BlockParserParameters;
            var closers = parameters.Closers;
            BlockCloserDelegate closer;

            info.FindFirstNonspace();

            if (info.IsBlank && info.Container.LastChild != null)
            {
                info.Container.LastChild.IsLastLineBlank = true;
            }

            info.Container.IsLastLineBlank = info.IsBlank && !parameters.IsDiscardLastBlank(info);

            Block cont = info.Container;
            while (cont.Parent != null)
            {
                cont.Parent.IsLastLineBlank = false;
                cont = cont.Parent;
            }

            if (!parameters.OpenParagraph(ref info))
            {
                // not a lazy continuation

                // finalize any blocks that were not matched and set cur to container:
                while (info.CurrentContainer != info.LastMatchedContainer)
                {
                    Finalize(info.CurrentContainer, info.LineInfo, settings);
                    info.CurrentContainer = info.CurrentContainer.Parent;

                    if (info.CurrentContainer == null)
                        throw new CommonMarkException("Cannot finalize container block. Last matched container tag = " + info.LastMatchedContainer.Tag);
                }

                if ((closer = closers[(int)info.Container.Tag]) != null && closer(info))
                {
                    // The closer will do the job
                }
                else
                {
                    parameters.CloseParagraph(info);
                }

                info.ParentContainer = info.Container;
            }
        }

        #endregion Close
    }
}
