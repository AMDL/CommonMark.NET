using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.BlockQuote"/> element formatter.
    /// </summary>
    public sealed class BlockQuoteFormatter : BlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockQuoteFormatter"/> class.
        /// </summary>
        /// <param name="parameters"></param>
        public BlockQuoteFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.BlockQuote, "blockquote", textTag: "block_quote")
        {
        }
    }
}
