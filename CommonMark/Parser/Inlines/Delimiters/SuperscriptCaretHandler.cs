using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines.Delimiters
{
    class SuperscriptCaretHandler : InlineDelimiterHandler
    {
        public override InlineTag GetTag(int delimiterCount)
        {
            return delimiterCount == 1 ? InlineTag.Superscript : 0;
        }
    }
}
