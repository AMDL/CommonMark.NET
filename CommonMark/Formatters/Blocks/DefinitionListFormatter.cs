using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class DefinitionListFormatter : BlockFormatter
    {
        public DefinitionListFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.DefinitionList;
        }

        protected override string GetTag(Block element)
        {
            return "dl";
        }
    }
}
