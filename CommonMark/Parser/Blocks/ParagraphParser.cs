using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.Paragraph"/> element parser.
    /// </summary>
    public class ParagraphParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public ParagraphParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the value indicating whether a handled element accepts new lines.
        /// </summary>
        /// <value><c>true</c> if new lines can be added to a handled element.</value>
        public override bool IsAcceptsLines
        {
            get { return true; }
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            Block cur = info.CurrentContainer;
            if (cur != info.LastMatchedContainer &&
                info.Container == info.LastMatchedContainer &&
                !info.IsBlank &&
                cur.Tag == BlockTag.Paragraph &&
                cur.StringContent.Length > 0)
            {
                // create lazy continuation paragraph
                AddLine(cur, info.LineInfo, info.Line, info.Offset);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Close(BlockParserInfo info)
        {
            if (info.IsBlank)
                return false;

            if (info.Container.Tag != BlockTag.Paragraph)
            {
                // create paragraph container for line
                info.Container = CreateChildBlock(info, BlockTag.Paragraph, info.FirstNonspace);
            }
            
            AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace);

            return true;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            var sc = container.StringContent;
            if (sc.StartsWith('['))
            {
                var subj = new Subject(container.Top.ReferenceMap);
                sc.FillSubject(subj);
                var origPos = subj.Position;
                while (subj.Position < subj.Buffer.Length
                    && subj.Buffer[subj.Position] == '['
                    && 0 != InlineMethods.ParseReference(subj, Settings.InlineParserParameters))
                {
                }

                if (subj.Position != origPos)
                {
                    sc.Replace(subj.Buffer, subj.Position, subj.Buffer.Length - subj.Position);

                    if (sc.PositionTracker != null)
                        sc.PositionTracker.AddBlockOffset(subj.Position - origPos);

                    if (Utilities.IsFirstLineBlank(subj.Buffer, subj.Position))
                        container.Tag = BlockTag.ReferenceDefinition;
                }
            }
            return true;
        }

        /// <summary>
        /// Processes the inline contents of a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Process(Block container, Subject subject, ref Stack<Inline> inlineStack)
        {
            return ProcessInlines(container, subject, ref inlineStack, Settings.InlineParserParameters);
        }
    }
}
