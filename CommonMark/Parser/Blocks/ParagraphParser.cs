﻿using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.Paragraph"/> element parser.
    /// </summary>
    public sealed class ParagraphParser : BlockParser, IBlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public ParagraphParser(CommonMarkSettings settings)
            : base(settings, BlockTag.Paragraph)
        {
            IsAcceptsLines = true;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get { yield return this; }
        }

        char IBlockDelimiterHandler.Character
        {
            get;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public bool Handle(ref BlockParserInfo info)
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
                info.Container = CreateChildBlock(info, Tag, info.FirstNonspace, Settings);
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
                var subj = new Subject(container.Top.DocumentData);
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