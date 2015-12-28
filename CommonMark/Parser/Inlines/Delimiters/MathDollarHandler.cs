namespace CommonMark.Parser.Inlines.Delimiters
{
    internal sealed class MathDollarHandler : InlineDelimiterHandler
    {
        public MathDollarHandler()
            : base('$', Syntax.InlineTag.Math)
        {
        }

        public override bool IsCanClose(Subject subject, int startIndex, int length, CharacterType before, CharacterType after, bool canOpen)
        {
            return !after.IsDigit;
        }
    }
}
