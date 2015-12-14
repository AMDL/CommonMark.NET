using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockFinalizer
    {
        private readonly BlockFinalizerDelegate inner;
        private readonly BlockFinalizerDelegate outer;

        public DelegateBlockFinalizer(BlockFinalizerDelegate inner, BlockFinalizerDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        private bool Finalize(ref Block container, LineInfo line, string ln, int first_nonspace, int offset, bool indented, bool blank, CommonMarkSettings settings)
        {
            return inner(ref container, line, ln, first_nonspace, offset, indented, blank, settings)
                || outer(ref container, line, ln, first_nonspace, offset, indented, blank, settings);
        }

        public static BlockFinalizerDelegate Merge(BlockFinalizerDelegate inner, BlockFinalizerDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockFinalizer(inner, outer).Finalize
                : inner;
        }
    }
}
