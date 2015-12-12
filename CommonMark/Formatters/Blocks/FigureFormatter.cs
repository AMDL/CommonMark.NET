using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class FigureFormatter : BlockFormatter
    {
        public FigureFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Figure;
        }

        protected override string GetTag(Block element)
        {
            return "figure";
        }
    }
}
