using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class SubscriptFormatter : InlineFormatter
    {
        public SubscriptFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Inline element)
        {
            return element.Tag == InlineTag.Subscript;
        }

        protected override string GetTag(Inline element)
        {
            return "sub";
        }
    }
}
