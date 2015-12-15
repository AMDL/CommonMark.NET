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

        private bool Initialize(ref BlockParserInfo info)
        {
            return inner(ref info)
                || outer(ref info);
        }

        public static BlockParserDelegate Merge(BlockParserDelegate inner, BlockParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockParser(inner, outer).Initialize
                : inner;
        }

        public static BlockParserDelegate Merge(params BlockParserDelegate[] delegates)
        {
            if (delegates == null || delegates.Length == 0)
                return null;

            if (delegates.Length == 1)
                return delegates[0];

            var skip1 = new BlockParserDelegate[delegates.Length - 1];
            System.Array.Copy(delegates, 1, skip1, 0, delegates.Length - 1);

            return Merge(delegates[0], Merge(skip1));
        }
    }
}
