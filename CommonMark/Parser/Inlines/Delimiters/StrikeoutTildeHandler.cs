using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    internal sealed class StrikeoutTildeHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 2 ? InlineTag.Strikethrough : 0;
        }

        public override bool Handle(Subject subject, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            return delimiterCount == 2;
        }
    }
}
