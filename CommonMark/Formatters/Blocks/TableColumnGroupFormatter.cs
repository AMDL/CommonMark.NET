using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters.Blocks
{
    internal class TableColumnGroupFormatter : BlockFormatter
    {
        public TableColumnGroupFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableColumnGroup;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant("<colgroup");
            WritePosition(writer, block);
            writer.WriteLineConstant(" />");
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</colgroup>";
        }

        public override string GetNodeTag(Block block)
        {
            return "table_colgroup";
        }

        public override void Print(TextWriter writer, Block block)
        {
        }
    }
}
