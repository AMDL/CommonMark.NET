using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    internal sealed class TableBodyCellFormatter : BlockFormatter
    {
        public TableBodyCellFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableCell && block.Parent.Tag == BlockTag.TableBody;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
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

            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        protected override string GetTag(Block element)
        {
            return "td";
        }

        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block block)
        {
            return new Dictionary<string, object>
            {
                {"align", block.TableCellData.ColumnData.Alignment},
            };
        }
    }

    internal sealed class TableHeaderCellFormatter : BlockFormatter
    {
        public TableHeaderCellFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block element)
        {
            return element.Tag == BlockTag.TableCell && element.Parent.Tag != BlockTag.TableBody;
        }

        protected override string GetTag(Block element)
        {
            return "th";
        }
    }

    internal sealed class TableCellFormatter : DelegateBlockFormatter
    {
        public TableCellFormatter(FormatterParameters parameters)
            : base(new TableBodyCellFormatter(parameters), new TableHeaderCellFormatter(parameters))
        {
        }
    }
}
