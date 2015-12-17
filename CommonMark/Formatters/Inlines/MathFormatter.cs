using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class MathFormatter : InlineFormatter
    {
        public MathFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Inline element)
        {
            return element.Tag == InlineTag.Math;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Inline element, bool withinLink)
        {
            writer.WriteConstant("<span class=\"math\"");
            WritePosition(writer, element);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(IHtmlFormatter formatter, Inline element, bool withinLink)
        {
            return "</span>";
        }

        public override string GetPrinterTag(Inline element)
        {
            return "math";
        }

        protected override string GetTag(Inline element)
        {
            throw new System.NotImplementedException();
        }
    }
}
