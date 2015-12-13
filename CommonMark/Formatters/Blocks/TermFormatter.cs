using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TermFormatter : BlockFormatter
    {
        public TermFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Term;
        }

        protected override string GetTag(Block element)
        {
            return "dt";
        }
    }
}
