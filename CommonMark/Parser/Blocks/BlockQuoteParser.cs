using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.BlockQuote"/> element parser.
    /// </summary>
    public sealed class BlockQuoteParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockQuoteParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public BlockQuoteParser(CommonMarkSettings settings)
            : base(settings, BlockTag.BlockQuote)
        {
            // block quote lines are never blank as they start with >
            IsAlwaysDiscardBlanks = true;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                yield return new Delimiters.BlockQuoteHandler(Settings, Tag);
            }
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
    }
}
