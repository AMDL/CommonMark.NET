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

        public bool IsCanOpen(Subject subj, int startpos, int len, CharacterType before, CharacterType after, bool canClose)
        {
            return inner.IsCanOpen(subj, startpos, len, before, after, canClose)
                || outer.IsCanOpen(subj, startpos, len, before, after, canClose);
        }

        public bool IsCanClose(Subject subj, int startpos, int len, CharacterType before, CharacterType after, bool canOpen)
        {
            return inner.IsCanClose(subj, startpos, len, before, after, canOpen)
                || outer.IsCanClose(subj, startpos, len, before, after, canOpen);
        }

        public static IInlineDelimiterHandler Merge(IInlineDelimiterHandler inner, IInlineDelimiterHandler outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateInlineDelimiterHandler(inner, outer)
                : outer;
        }
    }
}
