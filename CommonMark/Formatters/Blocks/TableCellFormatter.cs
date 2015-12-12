using CommonMark.Syntax;
using System.Collections.Generic;

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
            if (block.Parent.Tag == BlockTag.TableBody)
            {
                writer.WriteConstant("<td");
                switch (block.TableCellData.ColumnData.Alignment)
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
            }
            else
            {
                writer.WriteConstant("<th");
            }

            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        protected override string GetTag(Block element)
        {
            return element.Parent.Tag == BlockTag.TableBody
                ? "td"
                : "th";
        }

        public override IDictionary<string, object> GetPrinterData(Block block)
        {
            if (block.Parent.Tag != BlockTag.TableBody)
                return base.GetPrinterData(block);
            return new Dictionary<string, object>
            {
                {"align", block.TableCellData.ColumnData.Alignment},
            };
        }
    }
}
