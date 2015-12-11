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

        public Syntax.Inline ParseInline(Syntax.Block block, Parser.Subject subject)
        {
            return inner(block, subject) ?? outer(block, subject);
        }
    }
}
