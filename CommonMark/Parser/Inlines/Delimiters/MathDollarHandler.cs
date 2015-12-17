using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    internal sealed class MathDollarHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 1 ? InlineTag.Math : 0;
        }

        public override bool Handle(Subject subject, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            if (delimiterCount != 1)
                return false;
            canClose &= !char.IsDigit(subject.Buffer[startIndex]);
            return true;
        }
    }
}
