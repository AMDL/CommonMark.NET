using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class DefinitionFormatter : BlockFormatter
    {
        public DefinitionFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Definition;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<dd");
            WritePosition(writer, block);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</dd>";
        }

        public override string GetNodeTag(Block block)
        {
            return "definition";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
