using CommonMark.Syntax;
using System.Collections.Generic;

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

        public string GetClosing(IHtmlFormatter formatter, T element)
        {
            return inner.CanHandle(element)
                ? inner.GetClosing(formatter, element)
                : outer.GetClosing(formatter, element);
        }

        public string GetPrinterTag(T element)
        {
            return inner.CanHandle(element)
                ? inner.GetPrinterTag(element)
                : outer.GetPrinterTag(element);
        }

        public IDictionary<string, object> GetPrinterData(IPrinter printer, T element)
        {
            return inner.CanHandle(element)
                ? inner.GetPrinterData(printer, element)
                : outer.GetPrinterData(printer, element);
        }
    }

    internal class DelegateBlockFormatter : DelegateFormatter<Block, IBlockFormatter>, IBlockFormatter
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

        public static IBlockFormatter Merge(IBlockFormatter inner, IBlockFormatter outer)
        {
            return !inner.Equals(outer)
                ? new DelegateBlockFormatter(inner, outer)
                : inner;
        }
    }

    internal class DelegateInlineFormatter : DelegateFormatter<Inline, IInlineFormatter>, IInlineFormatter
    {
        public DelegateInlineFormatter(IInlineFormatter inner, IInlineFormatter outer)
            : base(inner, outer)
        {
        }

        public bool? IsRenderPlainTextInlines(Inline inline, bool plaintext)
        {
            return inner.CanHandle(inline)
                ? inner.IsRenderPlainTextInlines(inline, plaintext)
                : outer.IsRenderPlainTextInlines(inline, plaintext);
        }

        public static IInlineFormatter Merge(IInlineFormatter inner, IInlineFormatter outer)
        {
            return !inner.Equals(outer)
                ? new DelegateInlineFormatter(inner, outer)
                : inner;
        }
    }
}
