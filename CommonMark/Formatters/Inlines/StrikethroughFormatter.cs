using CommonMark.Syntax;

namespace CommonMark.Formatters.Inlines
{
    internal sealed class StrikethroughFormatter : InlineFormatter
    {
        public StrikethroughFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Inline inline)
        {
            return inline.Tag == InlineTag.Strikethrough;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Inline inline)
        {
            writer.WriteConstant("<del");
            WritePosition(writer, inline);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(Inline inline)
        {
            return "</del>";
        }

        public override string GetNodeTag(Inline inline)
        {
            return "del";
        }

        public override void Print(System.IO.TextWriter writer, Inline inline)
        {
        }
    }
}
