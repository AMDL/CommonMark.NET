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

        public string[] HtmlTags
        {
            get
            {
                return inner.HtmlTags
                    ?? outer.HtmlTags;
            }
        }

        public bool IsFixedOpening
        {
            get
            {
                return inner.IsFixedOpening
                    && outer.IsFixedOpening;
            }
        }

        public bool IsSelfClosing
        {
            get
            {
                return inner.IsSelfClosing
                    && outer.IsSelfClosing;
            }
        }

        public string TextTag
        {
            get
            {
                return inner.TextTag
                    ?? outer.TextTag;
            }
        }

        public bool? IsFixedHtmlInlines
        {
            get
            {
                return inner.IsFixedHtmlInlines == outer.IsFixedHtmlInlines
                    ? inner.IsFixedHtmlInlines
                    : null;
            }
        }

        public bool CanHandle(TElement element)
        {
            return inner.CanHandle(element)
                || outer.CanHandle(element);
        }

        public bool WriteOpening(IHtmlTextWriter writer, TElement element)
        {
            return inner.CanHandle(element)
                ? inner.WriteOpening(writer, element)
                : outer.WriteOpening(writer, element);
        }

        public string GetClosing(TElement element)
        {
            return inner.CanHandle(element)
                ? inner.GetClosing(element)
                : outer.GetClosing(element);
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
            get
            {
                return inner.IsList
                    || outer.IsList;
            }
        }

        public bool IsListItem
        {
            get
            {
                return inner.IsListItem
                    || outer.IsListItem;
            }
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

        public bool WritePlaintextOpening(IHtmlTextWriter writer, Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.WritePlaintextOpening(writer, inline)
                : outer.WritePlaintextOpening(writer, inline);
        }

        public string GetPlaintextClosing(Inline inline)
        {
            return inner.CanHandle(inline)
                ? inner.GetPlaintextClosing(inline)
                : outer.GetPlaintextClosing(inline);
        }

        public bool IsPlaintextInlines(Inline element)
        {
            return inner.CanHandle(element)
                ? inner.IsPlaintextInlines(element)
                : outer.IsPlaintextInlines(element);
        }

        public string Infix
        {
            get
            {
                return inner.Infix
                    ?? outer.Infix;
            }
        }

        public bool? IsFixedPlaintextInlines
        {
            get
            {
                return inner.IsFixedPlaintextInlines == outer.IsFixedPlaintextInlines
                    ? inner.IsFixedPlaintextInlines
                    : null;
            }
        }
    }
}
