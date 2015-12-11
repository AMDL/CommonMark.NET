﻿using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    internal class TableColumnFormatter : BlockFormatter
    {
        public TableColumnFormatter(CommonMarkSettings settings)
            : base(settings)
        {
        }

        public override bool CanHandle(Block block)
        {
            return block.Tag == BlockTag.TableColumn;
        }

        public override bool WriteOpening(IHtmlTextWriter writer, Block block)
        {
            writer.WriteConstant("<col");
            TableColumnData columnData = null;
            switch (columnData.Alignment)
            {
                case TableColumnAlignment.Left:
                    writer.WriteConstant(" style=\"text-align: left\"");
                    break;
                case TableColumnAlignment.Right:
                    writer.WriteConstant(" style=\"text-align: right\"");
                    break;
                case TableColumnAlignment.Center:
                    writer.WriteConstant(" style=\"text-align: center\"");
                    break;
            }
            WritePosition(writer, block);
            writer.WriteLineConstant(" />");
            return false;
        }

        public override string GetClosing(Block block)
        {
            return null;
        }
    }
}