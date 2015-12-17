using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    internal sealed class SubscriptTildeHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 1 ? InlineTag.Subscript : 0;
        }
    }
}
