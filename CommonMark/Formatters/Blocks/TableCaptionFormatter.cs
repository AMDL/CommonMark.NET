using CommonMark.Syntax;
using System.IO;

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

        public override bool? IsStackTight(Block block, bool tight)
        {
            return false;
        }

        public override string GetNodeTag(Block block)
        {
            return "table_caption";
        }

        public override void Print(TextWriter writer, Block block)
        {
            writer.Write(" (type={0})", block.TableCaptionData.CaptionType);
        }
    }
}
