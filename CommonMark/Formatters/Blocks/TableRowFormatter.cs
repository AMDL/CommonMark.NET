﻿using CommonMark.Syntax;
using System.IO;

namespace CommonMark.Formatters.Blocks
{
    internal class TableRowFormatter : BlockFormatter
    {
        public TableRowFormatter(FormatterParameters parameters)
            : base(parameters)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableRow;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<tr");
            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</tr>";
        }

        public override string GetNodeTag(Block block)
        {
            return "table_row";
        }

        public override void Print(TextWriter writer, Block block)
        {
        }
    }
}
