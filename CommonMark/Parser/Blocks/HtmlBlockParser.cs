using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.HtmlBlock"/> element parser.
    /// </summary>
    public class HtmlBlockParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlockParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public HtmlBlockParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the value indicating whether a handled element is a code block.
        /// </summary>
        /// <value><c>true</c> if a handled element is a code block.</value>
        public override bool IsCodeBlock
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get
            {
                return new[] { '<' };
            }
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
        {
            // all other block types can accept blanks
            if (info.IsBlank && info.Container.HtmlBlockType >= HtmlBlockType.InterruptingBlock)
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
            int matched;
            if (!info.IsIndented && (0 != (matched = (int)Scanner.scan_html_block_start(info.Line, info.FirstNonspace, info.Line.Length))
                || (info.Container.Tag != BlockTag.Paragraph && 0 != (matched = (int)Scanner.scan_html_block_start_7(info.Line, info.FirstNonspace, info.Line.Length)))))
            {
                info.Container = CreateChildBlock(info, BlockTag.HtmlBlock, info.FirstNonspace);
                info.Container.HtmlBlockType = (HtmlBlockType)matched;
                // note, we don't adjust offset because the tag is part of the text
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
            AddLine(info.Container, info.LineInfo, info.Line, info.Offset);

            if (Scanner.scan_html_block_end(info.Container.HtmlBlockType, info.Line, info.FirstNonspace, info.Line.Length))
            {
                BlockMethods.Finalize(info.Container, info.LineInfo, Settings);
                info.Container = info.Container.Parent;
            }

            return true;
        }
    }
}
