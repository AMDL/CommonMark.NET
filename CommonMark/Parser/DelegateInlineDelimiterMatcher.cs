namespace CommonMark.Parser
{
    internal sealed class DelegateInlineDelimiterMatcher
    {
        private InlineDelimiterHandlerDelegate inner;
        private InlineDelimiterHandlerDelegate outer;

        public DelegateInlineDelimiterMatcher(InlineDelimiterHandlerDelegate inner, InlineDelimiterHandlerDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        private bool Match(Subject subj, int numdelims, int startIndex, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            return inner(subj, numdelims, startIndex, length, beforeIsPunctuation, afterIsPunctuation, ref canOpen, ref canClose)
                || outer(subj, numdelims, startIndex, length, beforeIsPunctuation, afterIsPunctuation, ref canOpen, ref canClose);
        }

        public static InlineDelimiterHandlerDelegate Merge(InlineDelimiterHandlerDelegate inner, InlineDelimiterHandlerDelegate outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateInlineDelimiterMatcher(inner, outer).Match
                : outer;
        }
    }
}
