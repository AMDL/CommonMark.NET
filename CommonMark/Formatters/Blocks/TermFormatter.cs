using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TermFormatter : BlockFormatter
    {
        public TermFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Term;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<dt");
            WritePosition(writer, block);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</dt>";
        }

        public override string GetNodeTag(Block block)
        {
            return "term";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
