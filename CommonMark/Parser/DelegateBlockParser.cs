using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockParser : IBlockParser
    {
        private readonly IBlockParser inner;
        private readonly IBlockParser outer;

        public DelegateBlockParser(IBlockParser inner, IBlockParser outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public char[] Characters
        {
            get
            {
                var c = new List<char>(inner.Characters);
                c.AddRange(outer.Characters);
                return c.ToArray();
            }
        }

        public bool IsCodeBlock
        {
            get
            {
                return inner.IsCodeBlock
                    || outer.IsCodeBlock;
            }
        }

        public bool IsAcceptsLines
        {
            get
            {
                return inner.IsAcceptsLines
                    || outer.IsAcceptsLines;
            }
        }

        public bool IsDiscardLastBlank(BlockParserInfo info)
        {
            return inner.IsDiscardLastBlank(info)
                || outer.IsDiscardLastBlank(info);
        }

        public bool CanContain(BlockTag childTag)
        {
            return inner.CanContain(childTag)
                || outer.CanContain(childTag);
        }

        public bool Initialize(ref BlockParserInfo info)
        {
            return inner.Initialize(ref info)
                || outer.Initialize(ref info);
        }

        public bool Open(ref BlockParserInfo info)
        {
            return inner.Open(ref info) || outer.Open(ref info);
        }

        public bool Close(BlockParserInfo info)
        {
            return inner.Close(info)
                || outer.Close(info);
        }

        public bool Finalize(Block container)
        {
            return inner.Finalize(container)
                || outer.Finalize(container);
        }

        public bool Process(Block block, Subject subject, ref Stack<Inline> inlineStack)
        {
            return inner.Process(block, subject, ref inlineStack)
                || outer.Process(block, subject, ref inlineStack);
        }

        public static IBlockParser Merge(IBlockParser inner, IBlockParser outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockParser(inner, outer)
                : outer;
        }
    }
}
