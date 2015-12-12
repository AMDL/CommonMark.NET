using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal abstract class DelegateFormatter<T, TFormatter> : IElementFormatter<T>
        where TFormatter : IElementFormatter<T>
    {
        protected readonly TFormatter inner;
        protected readonly TFormatter outer;

        protected DelegateFormatter(TFormatter inner, TFormatter outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public bool CanHandle(T element)
        {
            return inner.CanHandle(element) || outer.CanHandle(element);
        }

        public bool WriteOpening(IHtmlTextWriter writer, T element)
        {
            return inner.CanHandle(element)
                ? inner.WriteOpening(writer, element)
                : outer.WriteOpening(writer, element);
        }

        public string GetClosing(T element)
        {
            return inner.CanHandle(element)
                ? inner.GetClosing(element)
                : outer.GetClosing(element);
        }

        public string GetPrinterTag(T element)
        {
            return inner.CanHandle(element)
                ? inner.GetPrinterTag(element)
                : outer.GetPrinterTag(element);
        }

        public System.Collections.Generic.IDictionary<string, object> GetPrinterData(T element)
        {
            return inner.CanHandle(element)
                ? inner.GetPrinterData(element)
                : outer.GetPrinterData(element);
        }
    }

    internal sealed class DelegateBlockFormatter : DelegateFormatter<Block, IBlockFormatter>, IBlockFormatter
    {
        public DelegateBlockFormatter(IBlockFormatter inner, IBlockFormatter outer)
            : base(inner, outer)
        {
        }

        public bool? IsStackTight(Block block, bool tight)
        {
            return inner.CanHandle(block)
                ? inner.IsStackTight(block, tight)
                : outer.IsStackTight(block, tight);
        }
    }

    internal sealed class DelegateInlineFormatter : DelegateFormatter<Inline, IInlineFormatter>, IInlineFormatter
    {
        public DelegateInlineFormatter(IInlineFormatter inner, IInlineFormatter outer)
            : base(inner, outer)
        {
        }

        public bool? IsRenderPlainTextInlines(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.IsRenderPlainTextInlines(inline)
                : outer.IsRenderPlainTextInlines(inline);
        }
    }
}
