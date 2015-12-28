using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    internal sealed class TableBodyCellFormatter : BlockFormatter
    {
        public TableBodyCellFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.TableBodyCell, "td")
        {
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant("<td");
            switch (block.TableCell.ColumnData.Alignment)
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

        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block block)
        {
            return new Dictionary<string, object>
            {
                {"align", block.TableCell.ColumnData.Alignment},
            };
        }
    }
}
