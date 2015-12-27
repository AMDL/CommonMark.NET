using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFormatter : BlockFormatter
    {
        public TableFormatter(FormatterParameters parameters)
            : base(parameters, BlockTag.Table, "table")
        {
        }

        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block block)
        {
            return new Dictionary<string, object>
            {
                {"type", block.Table.TableType},
                {"head_delim", block.Table.HeaderDelimiter},
                {"head_col_delim", block.Table.HeaderColumnDelimiter},
                {"col_delim", block.Table.ColumnDelimiter},
            };
        }
    }
}
