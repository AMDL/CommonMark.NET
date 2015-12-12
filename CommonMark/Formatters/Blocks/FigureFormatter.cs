﻿using CommonMark.Syntax;

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

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.EnsureLine();
            writer.WriteConstant("<figure");
            WritePosition(writer, block);
            writer.Write('>');
            return true;
        }

        public override string GetClosing(Block block)
        {
            return "</figure>";
        }

        public override string GetNodeTag(Block block)
        {
            return "figure";
        }

        public override void Print(System.IO.TextWriter writer, Block block)
        {
        }
    }
}
