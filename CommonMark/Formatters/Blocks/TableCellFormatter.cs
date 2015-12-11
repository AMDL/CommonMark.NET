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
            var data = block.TableCellData;
            writer.WriteConstant(data.CellType == TableCellType.Header ? "<th" : "<td");
            switch (data.ColumnData.Alignment)
            {
                case TableColumnAlignment.Left:
                    writer.WriteConstant(" style=\"text-align: left\"");
                    break;
                case TableColumnAlignment.Right:
                    writer.WriteConstant(" style=\"text-align: right\"");
                    break;
                case TableColumnAlignment.Center:
                    writer.WriteConstant(" style=\"text-align: center\"");
                    break;
            }
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
