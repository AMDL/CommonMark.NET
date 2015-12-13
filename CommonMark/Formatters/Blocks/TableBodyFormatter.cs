using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableBodyFormatter : BlockFormatter
    {
        public TableBodyFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableBody;
        }

        protected override string GetTag(Block element)
        {
            return "tbody";
        }
    }
}
