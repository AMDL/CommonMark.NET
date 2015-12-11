﻿using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableFormatter : BlockFormatter
    {
        public TableFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.Table;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<table");
            WritePosition(writer, block);
            writer.WriteLine('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</table>";
        }

        public override bool? IsStackTight(bool tight)
        {
            return false;
        }
    }
}