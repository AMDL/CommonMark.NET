using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFooterFormatter : BlockFormatter
    {
        public TableFooterFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableFooter;
        }

        protected override string GetTag(Block element)
        {
            return "tfoot";
        }
    }
}
