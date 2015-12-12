using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableBodyFormatter : BlockFormatter
    {
        public TableBodyFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableBody;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<tbody");
            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</tbody>";
        }

        public override string GetNodeTag(Block block)
        {
            return "table_body";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
