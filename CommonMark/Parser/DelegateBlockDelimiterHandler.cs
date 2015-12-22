﻿using CommonMark.Syntax;

namespace CommonMark.Parser
{
    class DelegateBlockDelimiterHandler : IBlockDelimiterHandler
    {
        private readonly IBlockDelimiterHandler inner;
        private readonly IBlockDelimiterHandler outer;

        private DelegateBlockDelimiterHandler(char[] characters, IBlockDelimiterHandler inner, IBlockDelimiterHandler outer)
        {
            this.Characters = characters;
            this.inner = inner;
            this.outer = outer;
        }

        public BlockTag Tag
        {
            get;
        }

        public char[] Characters
        {
            get;
        }

        public bool Handle(ref BlockParserInfo info)
        {
            return inner.Handle(ref info)
                || outer.Handle(ref info);
        }

        internal static IBlockDelimiterHandler Merge(IBlockDelimiterHandler inner, IBlockDelimiterHandler outer, params char[] characters)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockDelimiterHandler(characters, inner, outer)
                : outer;
        }
    }
}
