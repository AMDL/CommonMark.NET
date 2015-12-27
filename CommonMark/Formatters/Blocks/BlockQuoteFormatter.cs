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

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        public override bool? IsStackTight(Block element, bool tight)
        {
            return false;
        }
    }
}
