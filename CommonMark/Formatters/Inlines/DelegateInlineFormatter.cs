using CommonMark.Syntax;
using System.IO;

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

        public string GetNodeTag(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.GetNodeTag(inline)
                : outer.GetNodeTag(inline);
        }

        public void Print(TextWriter writer, Inline inline)
        {
            if (inner.CanHandle(inline))
                inner.Print(writer, inline);
            else
                outer.Print(writer, inline);
        }
    }
}
