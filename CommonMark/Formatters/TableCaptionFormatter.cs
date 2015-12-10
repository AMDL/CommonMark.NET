using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal class TableCaptionFormatter : BlockFormatter
    {
        public TableCaptionFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override void WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<caption");
            WritePosition(writer, block);
            writer.WriteLine('>');
        }

        public override string GetClosing(Block block, out bool visitChildren)
        {
            visitChildren = true;
            return "</caption>";
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}
