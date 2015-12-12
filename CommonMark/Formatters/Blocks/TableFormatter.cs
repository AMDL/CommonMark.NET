using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFormatter : BlockFormatter
    {
        public TableFormatter(FormatterParameters parameters)
            : base(parameters)
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
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</table>";
        }

        public override bool? IsStackTight(Block block, bool tight)
        {
            return false;
        }

        public override string GetNodeTag(Block block)
        {
            return "table";
        }

        public override void Print(TextWriter writer, Block block)
        {
            writer.Write(" (type={0} head_delim={1} head_col_delim={2} col_delim={3})",
                block.TableData.TableType,
                block.TableData.HeaderDelimiter,
                block.TableData.HeaderColumnDelimiter,
                block.TableData.ColumnDelimiter);
        }
    }
}
