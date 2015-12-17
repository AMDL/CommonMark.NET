using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableColumnFormatter : BlockFormatter
    {
        public TableColumnFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.TableColumn, "col")
        {
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant("<col");
            TableColumnData columnData = null;
            switch (columnData.Alignment)
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
            writer.WriteLineConstant(" />");
            return false;
        }

        public override string GetClosing(IHtmlFormatter formatter, Block block)
        {
            return null;
        }
    }
}
