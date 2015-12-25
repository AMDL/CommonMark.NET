using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters
{
    internal abstract class DelegateFormatter<TElement, TTag, TFormatter> : IElementFormatter<TElement, TTag>
        where TFormatter : IElementFormatter<TElement, TTag>
    {
        protected readonly TFormatter inner;
        protected readonly TFormatter outer;

        protected DelegateFormatter(TTag tag, TFormatter inner, TFormatter outer)
        {
            this.Tag = tag;
            this.inner = inner;
            this.outer = outer;
        }

        public TTag Tag
        {
            get;
        }

        public string PrinterTag
        {
            get { return inner.PrinterTag ?? outer.PrinterTag; }
        }

        public bool CanHandle(TElement element)
        {
            return inner.CanHandle(element) || outer.CanHandle(element);
        }

        public bool? IsRenderPlainTextInlines(TElement element, bool plaintext)
        {
            return inner.CanHandle(element)
                ? inner.IsRenderPlainTextInlines(element, plaintext)
                : outer.IsRenderPlainTextInlines(element, plaintext);
        }

        public IEnumerable<KeyValuePair<string, object>> GetPrinterData(IPrinter printer, TElement element)
        {
            return inner.CanHandle(element)
                ? inner.GetPrinterData(printer, element)
                : outer.GetPrinterData(printer, element);
        }
    }

    internal class DelegateBlockFormatter : DelegateFormatter<Block, BlockTag, IBlockFormatter>, IBlockFormatter
    {
        public DelegateBlockFormatter(BlockTag tag, IBlockFormatter inner, IBlockFormatter outer)
            : base(tag, inner, outer)
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
            return inner != null && !inner.Equals(outer)
                ? new DelegateBlockFormatter(BlockTag.Custom, inner, outer)
                : outer;
        }
    }

    internal class DelegateInlineFormatter : DelegateFormatter<Inline, InlineTag, IInlineFormatter>, IInlineFormatter
    {
        public DelegateInlineFormatter(InlineTag tag, IInlineFormatter inner, IInlineFormatter outer)
            : base(tag, inner, outer)
        {
        }

        public static IInlineFormatter Merge(IInlineFormatter inner, IInlineFormatter outer)
        {
            return inner != null && !inner.Equals(outer)
                ? new DelegateInlineFormatter(InlineTag.Custom, inner, outer)
                : outer;
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
