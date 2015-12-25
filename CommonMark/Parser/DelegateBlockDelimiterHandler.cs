using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal sealed class DelegateBlockDelimiterHandler : IBlockDelimiterHandler
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

        public override bool Equals(object obj)
        {
            var other = obj as DelegateBlockDelimiterHandler;
            return other != null
                && inner.Equals(other.inner)
                && outer.Equals(other.outer);
        }

        public override int GetHashCode()
        {
            return inner.GetHashCode()
                ^ outer.GetHashCode();
        }

        internal static IBlockDelimiterHandler Merge(IBlockDelimiterHandler inner, IBlockDelimiterHandler outer, params char[] characters)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockDelimiterHandler(characters, inner, outer)
                : outer;
        }

        internal static IBlockDelimiterHandler Merge(char[] characters, IBlockDelimiterHandler[] handlers)
        {
            if (handlers == null || handlers.Length == 0)
                return null;

            if (handlers.Length == 1)
                return handlers[0];

            var skip1 = new IBlockDelimiterHandler[handlers.Length - 1];
            System.Array.Copy(handlers, 1, skip1, 0, handlers.Length - 1);

            return Merge(handlers[0], Merge(characters, skip1), characters);
        }
    }
}
