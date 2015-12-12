using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    internal class TableCaptionFormatter : BlockFormatter
    {
        public TableCaptionFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableCaption;
        }

        protected override string GetTag(Block element)
        {
            return "caption";
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
