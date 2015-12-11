using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableCaptionFormatter : BlockFormatter
    {
        public TableCaptionFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableCaption;
        }

        public override void WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<caption");
            WritePosition(writer, block);
            writer.WriteLine('>');
        }

        public override string GetClosing(Block block, out bool visitChildren)
        {
            visitChildren = true;
            return "</caption>";
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}
