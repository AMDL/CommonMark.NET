﻿using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockParser : IBlockParser
    {
        private readonly IBlockParser inner;
        private readonly IBlockParser outer;

        public DelegateBlockParser(BlockTag tag, IBlockParser inner, IBlockParser outer)
        {
            this.Tag = tag;
            this.inner = inner;
            this.outer = outer;
        }

        public BlockTag Tag
        {
            get;
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

        public bool IsList
        {
            get
            {
                return inner.IsList
                    || outer.IsList;
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

        public bool IsAlwaysDiscardBlanks
        {
            get
            {
                return inner.IsAlwaysDiscardBlanks
                    || outer.IsAlwaysDiscardBlanks;
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

        public static IBlockParser Merge(BlockTag tag, IBlockParser inner, IBlockParser outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockParser(tag, inner, outer)
                : outer;
        }

        public static IBlockParser Merge(BlockTag tag, params IBlockParser[] parsers)
        {
            if (parsers == null || parsers.Length == 0)
                return null;

            if (parsers.Length == 1)
                return parsers[0];

            var skip1 = new IBlockParser[parsers.Length - 1];
            System.Array.Copy(parsers, 1, skip1, 0, parsers.Length - 1);

            return Merge(tag, parsers[0], Merge(tag, skip1));
        }
    }
}
