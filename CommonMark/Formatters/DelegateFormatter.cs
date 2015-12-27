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

        public string TextTag
        {
            get
            {
                return inner.TextTag
                    ?? outer.TextTag;
            }
        }

        public bool CanHandle(TElement element)
        {
            return inner.CanHandle(element)
                || outer.CanHandle(element);
        }

        public bool? IsRenderPlainTextInlines(TElement element)
        {
            return inner.CanHandle(element)
                ? inner.IsRenderPlainTextInlines(element)
                : outer.IsRenderPlainTextInlines(element);
        }

        public IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, TElement element)
        {
            return inner.CanHandle(element)
                ? inner.GetSyntaxData(formatter, element)
                : outer.GetSyntaxData(formatter, element);
        }
    }

    internal class DelegateBlockFormatter : DelegateFormatter<Block, BlockTag, IBlockFormatter>, IBlockFormatter
    {
        public DelegateBlockFormatter(BlockTag tag, IBlockFormatter inner, IBlockFormatter outer)
            : base(tag, inner, outer)
        {
        }

        public bool WriteOpening(IHtmlTextWriter writer, Block block, bool tight)
        {
            return inner.CanHandle(block)
                ? inner.WriteOpening(writer, block, tight)
                : outer.WriteOpening(writer, block, tight);
        }

        public string GetClosing(IHtmlFormatter formatter, Block block, bool tight)
        {
            return inner.CanHandle(block)
                ? inner.GetClosing(formatter, block, tight)
                : outer.GetClosing(formatter, block, tight);
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

        public bool WriteOpening(IHtmlTextWriter writer, Inline inline, bool plaintext, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.WriteOpening(writer, inline, plaintext, withinLink)
                : outer.WriteOpening(writer, inline, plaintext, withinLink);
        }

        public string GetClosing(IHtmlFormatter formatter, Inline inline, bool plaintext, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.GetClosing(formatter, inline, plaintext, withinLink)
                : outer.GetClosing(formatter, inline, plaintext, withinLink);
        }

        public bool IsStackWithinLink(Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.IsStackWithinLink(inline, withinLink)
                : outer.IsStackWithinLink(inline, withinLink);
        }
    }
}
