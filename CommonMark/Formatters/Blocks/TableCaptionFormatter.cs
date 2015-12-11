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

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<caption");
            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</caption>";
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}
