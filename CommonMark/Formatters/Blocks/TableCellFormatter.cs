using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters.Blocks
{
    internal class TableCellFormatter : BlockFormatter
    {
        public TableCellFormatter(FormatterParameters parameters)
            : base(parameters)
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

        public override bool? IsStackTight(Block block, bool tight)
        {
            return false;
        }

        public override string GetNodeTag(Block block)
        {
            return "table_cell";
        }

        public override void Print(TextWriter writer, Block block)
        {
            writer.Write(" (type={0} align={1})",
                block.TableCellData.CellType,
                block.TableCellData.ColumnData.Alignment);
        }
    }
}
