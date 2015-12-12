using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFooterFormatter : BlockFormatter
    {
        public TableFooterFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableFooter;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<tfoot");
            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</tfoot>";
        }

        public override string GetNodeTag(Block block)
        {
            return "table_foot";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
