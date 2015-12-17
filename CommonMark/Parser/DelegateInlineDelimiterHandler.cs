namespace CommonMark.Parser
{
    internal sealed class DelegateInlineDelimiterHandler : IInlineDelimiterHandler
    {
        private readonly IInlineDelimiterHandler inner;
        private readonly IInlineDelimiterHandler outer;

        public DelegateInlineDelimiterHandler(IInlineDelimiterHandler inner, IInlineDelimiterHandler outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public Syntax.InlineTag GetTag(int numdelims)
        {
            var tag = inner.GetTag(numdelims);
            return tag > 0 ? tag : outer.GetTag(numdelims);
        }

        public bool Handle(Subject subj, int numdelims, int startpos, int len, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
            return inner.Handle(subj, numdelims, startpos, len, beforeIsPunctuation, afterIsPunctuation, ref canOpen, ref canClose)
                || outer.Handle(subj, numdelims, startpos, len, beforeIsPunctuation, afterIsPunctuation, ref canOpen, ref canClose);
        }

        public static IInlineDelimiterHandler Merge(IInlineDelimiterHandler inner, IInlineDelimiterHandler outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateInlineDelimiterHandler(inner, outer)
                : outer;
        }
    }
}
