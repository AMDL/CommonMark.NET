using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableRowFormatter : BlockFormatter
    {
        public TableRowFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override void WriteOpening(IHtmlTextWriter writer, Block block)
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
        }

        public override string GetClosing(Block block, out bool visitChildren)
        {
            var rowType = block.TableRowData.TableRowType;

            var stackLiteral = "</tr>";

            if (0 != (rowType & TableRowType.Header))
                stackLiteral += "\n</thead>";
            else if (0 != (rowType & TableRowType.Footer))
                stackLiteral += "\n</tfoot>";
            else if (0 != (rowType & TableRowType.Last))
                stackLiteral += "\n</tbody>";

            visitChildren = true;
            return stackLiteral;
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}
