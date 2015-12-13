using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableColumnGroupFormatter : BlockFormatter
    {
        public TableColumnGroupFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableColumnGroup;
        }

        protected override string GetTag(Block element)
        {
            return "colgroup";
        }
    }
}
