using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockParser
    {
        private readonly BlockParserDelegate inner;
        private readonly BlockParserDelegate outer;

        public DelegateBlockParser(BlockParserDelegate inner, BlockParserDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public bool Parse(Block container, string line, int first_nonspace, bool indented, ref int offset, ref int column)
        {
            return inner(container, line, first_nonspace, indented, ref offset, ref column)
                || outer(container, line, first_nonspace, indented, ref offset, ref column);
        }

        public static BlockParserDelegate Merge(BlockParserDelegate inner, BlockParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockParser(inner, outer).Parse
                : inner;
        }
    }
}
