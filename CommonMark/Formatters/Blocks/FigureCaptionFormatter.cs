using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    class FigureCaptionFormatter : BlockFormatter
    {
        public FigureCaptionFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Figure;
        }

        protected override string GetTag(Block element)
        {
            return "figcaption";
        }

        public override IDictionary<string, object> GetPrinterData(Block block)
        {
            return new Dictionary<string, object>
            {
                {"placement", block.CaptionData.Placement},
                {"lead", block.CaptionData.Lead},
            };
        }
    }
}
