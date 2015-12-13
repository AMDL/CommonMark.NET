using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class DefinitionFormatter : BlockFormatter
    {
        public DefinitionFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Definition;
        }

        protected override string GetTag(Block element)
        {
            return "dd";
        }
    }
}
