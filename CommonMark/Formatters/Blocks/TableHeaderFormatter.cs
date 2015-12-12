using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableHeaderFormatter : BlockFormatter
    {
        public TableHeaderFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableHeader;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant("<thead>");
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</thead>";
        }

        public override string GetNodeTag(Block block)
        {
            return "table_head";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
