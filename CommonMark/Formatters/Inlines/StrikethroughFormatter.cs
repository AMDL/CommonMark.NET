using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class StrikethroughFormatter : InlineFormatter
    {
        public StrikethroughFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Inline inline)
        {
            return inline.Tag == InlineTag.Strikethrough;
        }

        protected override string GetTag(Inline element)
        {
            return "del";
        }
    }
}
