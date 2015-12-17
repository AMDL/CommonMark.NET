using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class MathFormatter : InlineFormatter
    {
        public MathFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Math)
        {
            PrinterTag = "math";
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
    }
}
