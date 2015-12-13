using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableRowFormatter : BlockFormatter
    {
        public TableRowFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableRow;
        }

        protected override string GetTag(Block element)
        {
            return "tr";
        }
    }
}
