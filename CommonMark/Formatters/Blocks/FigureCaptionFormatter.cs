using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    class FigureCaptionFormatter : BlockFormatter
    {
        public FigureCaptionFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Figure;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<figcaption");
            WritePosition(writer, block);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</figcaption>";
        }

        public override string GetNodeTag(Block block)
        {
            return "figure_caption";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
            writer.Write(" (placement={0} lead={1})",
                block.CaptionData.Placement,
                block.CaptionData.Lead);
        }
    }
}
