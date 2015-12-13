using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFormatter : BlockFormatter
    {
        public TableFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Table;
        }

        protected override string GetTag(Block element)
        {
            return "table";
        }

        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block block)
        {
            return new Dictionary<string, object>
            {
                {"type", block.TableData.TableType},
                {"head_delim", block.TableData.HeaderDelimiter},
                {"head_col_delim", block.TableData.HeaderColumnDelimiter},
                {"col_delim", block.TableData.ColumnDelimiter},
            };
        }
    }
}
