using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class MathFormatter : InlineFormatter
    {
        public MathFormatter(FormatterParameters parameters)
            : base(parameters, InlineTag.Math, "math", "span")
        {
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Inline element)
        {
            writer.WriteConstant("<span class=\"math\"");
            WritePosition(writer, element);
            writer.Write('>');
            return true;
        }
    }
}
