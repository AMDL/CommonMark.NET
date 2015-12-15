using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.SETextHeader"/> element parser.
    /// </summary>
    public class SETextHeaderParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public SETextHeaderParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get
            {
                return new[] { '-', '=' };
            }
        }

        /// <summary>
        /// Determines whether a blank line at the end of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool IsDiscardLastBlank(BlockParserInfo info)
        {
            // we don't count setext headers for purposes of tight/loose lists or breaking out of lists.
            return true;
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            // a header can never contain more than one line
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
            }
            return false;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
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

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Close(BlockParserInfo info)
        {
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
