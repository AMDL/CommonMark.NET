namespace CommonMark.Parser
{
    internal sealed class DelegateBlockAdvancer
    {
        private readonly BlockAdvancerDelegate inner;
        private readonly BlockAdvancerDelegate outer;

        public DelegateBlockAdvancer(BlockAdvancerDelegate inner, BlockAdvancerDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        private bool Advance(Syntax.Block container, string line, int first_nonspace, ref int offset, ref int column, int indent, bool blank)
        {
            return inner(container, line, first_nonspace, ref offset, ref column, indent, blank)
                || outer(container, line, first_nonspace, ref offset, ref column, indent, blank);
        }

        public static BlockAdvancerDelegate Merge(BlockAdvancerDelegate inner, BlockAdvancerDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockAdvancer(inner, outer).Advance
                : inner;
        }
    }
}
