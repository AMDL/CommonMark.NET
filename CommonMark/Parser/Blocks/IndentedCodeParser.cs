using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.IndentedCode"/> element parser.
    /// </summary>
    public sealed class IndentedCodeParser : BlockParser, IBlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndentedCodeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public IndentedCodeParser(CommonMarkSettings settings)
            : base(settings, BlockTag.IndentedCode)
        {
            IsCodeBlock = true;
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

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public bool Handle(ref BlockParserInfo info)
        {
            if (info.IsIndented && !info.IsMaybeLazy && !info.IsBlank)
            {
                info.AdvanceIndentedOffset();
                info.Container = CreateChildBlock(info, Tag, info.Offset, Settings);
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
            AddLine(info.Container, info.LineInfo, info.Line, info.Offset);
            return true;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            container.StringContent.RemoveTrailingBlankLines();
            return true;
        }
    }
}
