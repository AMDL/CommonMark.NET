using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    class SuperscriptCaretHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 1 ? InlineTag.Superscript : 0;
        }

        public override bool Handle(Subject subject, int delimiterCount, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            return delimiterCount == 1;
        }
    }
}
