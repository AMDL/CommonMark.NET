using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableHeaderFormatter : BlockFormatter
    {
        public TableHeaderFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableHeader;
        }

        protected override string GetTag(Block element)
        {
            return "thead";
        }
    }
}
