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

        public bool Process(Syntax.Block container, Subject subject, Dictionary<string, Reference> referenceMap, CommonMarkSettings settings, ref Stack<Inline> inlineStack)
        {
            return inner(container, subject, referenceMap, settings, ref inlineStack)
                || outer(container, subject, referenceMap, settings, ref inlineStack);
        }
    }
}
