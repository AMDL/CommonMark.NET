using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockInitializer
    {
        private readonly BlockInitializerDelegate inner;
        private readonly BlockInitializerDelegate outer;

        public DelegateBlockInitializer(BlockInitializerDelegate inner, BlockInitializerDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        private bool Initialize(ref Block container, LineInfo lineInfo, string line, int first_nonspace, int indent, bool indented, bool all_matched, ref int offset, ref int column, CommonMarkSettings settings)
        {
            return inner(ref container, lineInfo, line, first_nonspace, indent, indented, all_matched, ref offset, ref column, settings)
                || outer(ref container, lineInfo, line, first_nonspace, indent, indented, all_matched, ref offset, ref column, settings);
        }

        public static BlockInitializerDelegate Merge(BlockInitializerDelegate inner, BlockInitializerDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockInitializer(inner, outer).Initialize
                : inner;
        }

        public static BlockInitializerDelegate Merge(params BlockInitializerDelegate[] delegates)
        {
            if (delegates == null || delegates.Length == 0)
                return null;

            if (delegates.Length == 1)
                return delegates[0];

            var skip1 = new BlockInitializerDelegate[delegates.Length - 1];
            System.Array.Copy(delegates, 1, skip1, 0, delegates.Length - 1);

            return Merge(delegates[0], Merge(skip1));
        }
    }
}
