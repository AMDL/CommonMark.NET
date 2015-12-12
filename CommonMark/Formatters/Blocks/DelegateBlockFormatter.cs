using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    internal sealed class DelegateBlockFormatter : IBlockFormatter
    {
        private readonly IBlockFormatter inner;
        private readonly IBlockFormatter outer;
        
        public DelegateBlockFormatter(IBlockFormatter inner, IBlockFormatter outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public bool CanHandle(Block block)
        {
            return inner.CanHandle(block) || outer.CanHandle(block);
        }

        public bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            return inner.CanHandle(block)
                ? inner.WriteOpening(writer, block)
                : outer.WriteOpening(writer, block);
        }

        public string GetClosing(Block block)
        {
            return inner.CanHandle(block)
                ? inner.GetClosing(block)
                : outer.GetClosing(block);
        }

        public bool? IsStackTight(Block block, bool tight)
        {
            return inner.CanHandle(block)
                ? inner.IsStackTight(block, tight)
                : outer.IsStackTight(block, tight);
        }

        public string GetPrinterTag(Block block)
        {
            return inner.CanHandle(block)
                ? inner.GetPrinterTag(block)
                : outer.GetPrinterTag(block);
        }

        public IDictionary<string, object> GetPrinterData(Block block)
        {
            return inner.CanHandle(block)
                ? inner.GetPrinterData(block)
                : outer.GetPrinterData(block);
        }
    }
}
