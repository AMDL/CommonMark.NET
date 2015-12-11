using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableCellFormatter : BlockFormatter
    {
        public TableCellFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableCell;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant(block.TableCellData.CellType == TableCellType.Header ? "<th" : "<td");
            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
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
