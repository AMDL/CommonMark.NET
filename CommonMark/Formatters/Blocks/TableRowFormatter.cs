using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters.Blocks
{
    internal class TableRowFormatter : BlockFormatter
    {
        public TableRowFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableRow;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            var rowType = block.TableRowData.TableRowType;

            writer.EnsureLine();

            if (0 != (rowType & TableRowType.Header))
                writer.WriteConstant("<thead>");
            else if (0 != (rowType & TableRowType.Footer))
                writer.WriteConstant("<tfoot>");
            else if (0 != (rowType & TableRowType.First))
                writer.WriteConstant("<tbody>");

            writer.EnsureLine();

            writer.WriteConstant("<tr");
            WritePosition(writer, block);
            writer.WriteLine('>');

            return true;
        }

        public override string GetClosing(Block block)
        {
            var rowType = block.TableRowData.TableRowType;

            var stackLiteral = "</tr>";

            if (0 != (rowType & TableRowType.Header))
                stackLiteral += "\n</thead>";
            else if (0 != (rowType & TableRowType.Footer))
                stackLiteral += "\n</tfoot>";
            else if (0 != (rowType & TableRowType.Last))
                stackLiteral += "\n</tbody>";

            return stackLiteral;
        }

        public override bool? IsStackTight(Block block, bool tight)
        {
            return false;
        }

        public override string GetNodeTag(Block block)
        {
            return "table_row";
        }

        public override void Print(TextWriter writer, Block block)
        {
            writer.Write(" (type={0})", block.TableRowData.TableRowType);
        }
    }
}
