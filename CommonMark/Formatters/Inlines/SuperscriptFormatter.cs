using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class SuperscriptFormatter : InlineFormatter
    {
        public SuperscriptFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }
        
        public override bool CanHandle(Inline element)
        {
            return element.Tag == InlineTag.Superscript;
        }

        protected override string GetTag(Inline element)
        {
            return "sup";
        }
    }
}
