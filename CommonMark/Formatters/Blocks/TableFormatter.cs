using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFormatter : BlockFormatter
    {
        public TableFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Table;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<table");
            WritePosition(writer, block);
            writer.WriteLine('>');
            writer.WriteLineConstant("<colgroup>");
            for (var column = block.TableData.FirstColumn; column != null; column = column.NextSibling)
            {
                writer.WriteConstant("<col");
                switch (column.Alignment)
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
                writer.WriteLineConstant(" />");
            }
            writer.WriteLineConstant("</colgroup>");
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</table>";
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}
