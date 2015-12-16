using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.BlockQuote"/> element parser.
    /// </summary>
    public class BlockQuoteParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockQuoteParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public BlockQuoteParser(CommonMarkSettings settings)
            : base(settings, '>')
        {
        }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if blank lines at the end of the handled element should be discarded.</returns>
        public override bool IsDiscardLastBlank(BlockParserInfo info)
        {
            // block quote lines are never blank as they start with >
            return true;
        }

        /// <summary>
        /// Determines whether a handled element can contain child elements of the specified kind.
        /// </summary>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if handled elements can contain elements having <paramref name="childTag"/>.</returns>
        public override bool CanContain(BlockTag childTag)
        {
            return true;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
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

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
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
            info.Container = CreateChildBlock(info, BlockTag.BlockQuote, info.FirstNonspace);
            return true;
        }
    }
}
