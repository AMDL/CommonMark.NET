using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    internal sealed class MathDollarHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 1 ? InlineTag.Math : 0;
        }

        public override bool IsCanClose(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canOpen)
        {
            return !after.IsDigit;
        }
    }
}
