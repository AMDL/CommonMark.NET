using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableCellFormatter : BlockFormatter
    {
        public TableCellFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override void WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant(block.TableCellData.CellType == TableCellType.Header ? "<th" : "<td");
            WritePosition(writer, block);
            writer.WriteLine('>');
        }

        public override string GetClosing(Block block, out bool visitChildren)
        {
            visitChildren = true;
            return block.TableCellData.CellType == TableCellType.Header
                ? "</th>"
                : "</td>";
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}
