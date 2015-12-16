namespace CommonMark.Parser
{
    internal sealed class DelegateInlineHandler
    {
        private readonly InlineHandlerDelegate inner;
        private readonly InlineHandlerDelegate outer;

        public DelegateInlineHandler(InlineHandlerDelegate inner, InlineHandlerDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public Syntax.Inline Parse(Syntax.Block block, Parser.Subject subject)
        {
            return inner(block, subject)
                ?? outer(block, subject);
        }

        public static InlineHandlerDelegate Merge(InlineHandlerDelegate inner, InlineHandlerDelegate outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new Parser.DelegateInlineHandler(inner, outer).Parse
                : outer;
        }
    }
}
