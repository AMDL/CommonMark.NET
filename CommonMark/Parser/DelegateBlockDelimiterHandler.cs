using CommonMark.Syntax;

namespace CommonMark.Parser
{
    class DelegateBlockDelimiterHandler : IBlockDelimiterHandler
    {
        private readonly IBlockDelimiterHandler inner;
        private readonly IBlockDelimiterHandler outer;

        private DelegateBlockDelimiterHandler(char character, IBlockDelimiterHandler inner, IBlockDelimiterHandler outer)
        {
            this.Character = character;
            this.inner = inner;
            this.outer = outer;
        }

        public BlockTag Tag
        {
            get;
        }

        public char Character
        {
            get;
        }

        public bool Handle(ref BlockParserInfo info)
        {
            return inner.Handle(ref info)
                || outer.Handle(ref info);
        }

        internal static IBlockDelimiterHandler Merge(char character, IBlockDelimiterHandler inner, IBlockDelimiterHandler outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockDelimiterHandler(character, inner, outer)
                : outer;
        }
    }
}
