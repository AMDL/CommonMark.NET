using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class DefinitionListFormatter : BlockFormatter
    {
        public DefinitionListFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.DefinitionList;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<dl");
            WritePosition(writer, block);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</dl>";
        }

        public override string GetNodeTag(Block block)
        {
            return "definition_list";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
