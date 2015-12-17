using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    internal sealed class StrikeoutTildeHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 2 ? InlineTag.Strikethrough : 0;
        }
    }
}
