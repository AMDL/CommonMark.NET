namespace CommonMark.Parser
{
    internal sealed class DelegateInlineParser
    {
        private readonly InlineParserDelegate inner;
        private readonly InlineParserDelegate outer;

        public DelegateInlineParser(InlineParserDelegate inner, InlineParserDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public Syntax.Inline Parse(Syntax.Block block, Parser.Subject subject)
        {
            return inner(block, subject) ?? outer(block, subject);
        }

        public static InlineParserDelegate Merge(InlineParserDelegate inner, InlineParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new Parser.DelegateInlineParser(inner, outer).Parse
                : inner;
        }
    }
}
