using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class DelegateInlineFormatter : IInlineFormatter
    {
        private readonly IInlineFormatter inner;
        private readonly IInlineFormatter outer;

        public DelegateInlineFormatter(IInlineFormatter inner, IInlineFormatter outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public bool CanHandle(Inline inline)
        {
            return inner.CanHandle(inline) || outer.CanHandle(inline);
        }

        public bool WriteOpening(IHtmlTextWriter writer, Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.WriteOpening(writer, inline)
                : outer.WriteOpening(writer, inline);
        }

        public string GetClosing(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.GetClosing(inline)
                : outer.GetClosing(inline);
        }

        public bool? IsRenderPlainTextInlines(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.IsRenderPlainTextInlines(inline)
                : outer.IsRenderPlainTextInlines(inline);
        }

        public string GetPrinterTag(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.GetPrinterTag(inline)
                : outer.GetPrinterTag(inline);
        }

        public IDictionary<string, object> GetPrinterData(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.GetPrinterData(inline)
                : outer.GetPrinterData(inline);
        }
    }
}
