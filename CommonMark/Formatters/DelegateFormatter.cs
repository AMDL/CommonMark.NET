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

        public bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            return inner.CanHandle(block)
                ? inner.WriteOpening(writer, block)
                : outer.WriteOpening(writer, block);
        }

        public string GetClosing(IHtmlFormatter formatter, Block block)
        {
            return inner.CanHandle(block)
                ? inner.GetClosing(formatter, block)
                : outer.GetClosing(formatter, block);
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

        public bool WriteOpening(IHtmlTextWriter writer, Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.WriteOpening(writer, inline, withinLink)
                : outer.WriteOpening(writer, inline, withinLink);
        }

        public string GetClosing(IHtmlFormatter formatter, Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.GetClosing(formatter, inline, withinLink)
                : outer.GetClosing(formatter, inline, withinLink);
        }

        public bool IsStackWithinLink(Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.IsStackWithinLink(inline, withinLink)
                : outer.IsStackWithinLink(inline, withinLink);
        }
    }
}
