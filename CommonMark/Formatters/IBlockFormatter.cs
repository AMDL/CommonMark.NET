using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Block element formatter interface.
    /// </summary>
    public interface IBlockFormatter : IElementFormatter<Block, BlockTag>
    {
        /// <summary>
        /// Determines whether paragraph rendering should be skipped for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's rendering option.</param>
        /// <returns><c>true</c> to skip paragraph rendering.</returns>
        bool IsTight(Block element, bool tight);
    }
}
