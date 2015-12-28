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

        public bool WriteOpening(IHtmlTextWriter writer, TElement element, bool flag)
        {
            return inner.CanHandle(element)
                ? inner.WriteOpening(writer, element, flag)
                : outer.WriteOpening(writer, element, flag);
        }

        public string GetClosing(TElement element, bool withinLink)
        {
            return inner.CanHandle(element)
                ? inner.GetClosing(element, withinLink)
                : outer.GetClosing(element, withinLink);
        }

        public bool IsHtmlInlines(TElement element)
        {
            return inner.CanHandle(element)
                ? inner.IsHtmlInlines(element)
                : outer.IsHtmlInlines(element);
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

        public bool IsTight(Block block)
        {
            return inner.CanHandle(block)
                ? inner.IsTight(block)
                : outer.IsTight(block);
        }

        public bool IsList
        {
            get { return inner.IsList; }
        }

        public bool IsListItem
        {
            get { return inner.IsListItem; }
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

        public bool WritePlaintextOpening(IHtmlTextWriter writer, Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.WritePlaintextOpening(writer, inline, withinLink)
                : outer.WritePlaintextOpening(writer, inline, withinLink);
        }

        public string GetInfix(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.GetInfix(inline)
                : outer.GetInfix(inline);
        }

        public string GetPlaintextClosing(Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.GetPlaintextClosing(inline, withinLink)
                : outer.GetPlaintextClosing(inline, withinLink);
        }

        public bool IsPlaintextInlines(Inline element)
        {
            return inner.CanHandle(element)
                ? inner.IsPlaintextInlines(element)
                : outer.IsPlaintextInlines(element);
        }

        public bool IsWithinLink(Inline inline, bool withinLink)
        {
            return inner.CanHandle(inline)
                ? inner.IsWithinLink(inline, withinLink)
                : outer.IsWithinLink(inline, withinLink);
        }
    }
}
