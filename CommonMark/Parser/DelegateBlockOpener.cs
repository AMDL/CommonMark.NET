namespace CommonMark.Parser
{
    internal sealed class DelegateBlockOpener
    {
        private readonly BlockOpenerDelegate inner;
        private readonly BlockOpenerDelegate outer;

        public DelegateBlockOpener(BlockOpenerDelegate inner, BlockOpenerDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        private bool Initialize(ref BlockParserInfo info)
        {
            return inner(ref info)
                || outer(ref info);
        }

        public static BlockOpenerDelegate Merge(BlockOpenerDelegate inner, BlockOpenerDelegate outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockOpener(inner, outer).Initialize
                : outer;
        }

        private static BlockOpenerDelegate Merge(params BlockOpenerDelegate[] delegates)
        {
            if (delegates == null || delegates.Length == 0)
                return null;

            if (delegates.Length == 1)
                return delegates[0];

            var skip1 = new BlockOpenerDelegate[delegates.Length - 1];
            System.Array.Copy(delegates, 1, skip1, 0, delegates.Length - 1);

            return Merge(delegates[0], Merge(skip1));
        }
    }
}
