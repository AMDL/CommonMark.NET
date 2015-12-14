using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockProcessor
    {
        private readonly BlockProcessorDelegate inner;
        private readonly BlockProcessorDelegate outer;

        public DelegateBlockProcessor(BlockProcessorDelegate inner, BlockProcessorDelegate outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        private bool Process(Block container, Subject subject, Dictionary<string, Reference> referenceMap, ref Stack<Inline> inlineStack, CommonMarkSettings settings)
        {
            return inner(container, subject, referenceMap, ref inlineStack, settings)
                || outer(container, subject, referenceMap, ref inlineStack, settings);
        }

        public static BlockProcessorDelegate Merge(BlockProcessorDelegate inner, BlockProcessorDelegate outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockProcessor(inner, outer).Process
                : inner;
        }
    }
}
