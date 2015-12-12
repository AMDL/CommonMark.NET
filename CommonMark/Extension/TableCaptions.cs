using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Table captions.
    /// </summary>
    public class TableCaptions : CommonMarkExtension
    {
        private readonly TableCaptionsSettings settings;
        private readonly TableCaptionFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCaptions"/> class.
        /// </summary>
        /// <param name="settings">The object containing settings to be cloned for the formatting process.</param>
        /// <param name="tableCaptionsSettings">Table captions settings.</param>
        public TableCaptions(CommonMarkSettings settings, TableCaptionsSettings tableCaptionsSettings)
        {
            this.settings = tableCaptionsSettings;
            this.formatter = new TableCaptionFormatter(settings.FormatterParameters);
        }

        /// <summary>
        /// Gets the mapping from block tag to block element formatter.
        /// </summary>
        public override IDictionary<BlockTag, Formatters.IBlockFormatter> BlockFormatters
        {
            get
            {
                return new Dictionary<BlockTag, Formatters.IBlockFormatter>
                {
                    { BlockTag.TableCaption, formatter }
                };
            }
        }

        internal bool IncorporateCaption(Block block)
        {
            return ((0 != (settings.Features & TableCaptionsFeatures.After) && block.Tag == BlockTag.Table
                        && !HasCaption(block) && GetData(block.NextSibling) && IncorporateTrailing(block))
                    || (0 != (settings.Features & TableCaptionsFeatures.Before) && block.NextSibling?.NextSibling?.Tag == BlockTag.Table
                        && !HasCaption(block.NextSibling.NextSibling) && GetData(block.NextSibling) && IncorporateLeading(block)));
        }

        private static bool HasCaption(Block table)
        {
            for (var child = table.FirstChild; child != null; child = child.NextSibling)
                if (child.Tag == BlockTag.TableCaption)
                    return true;
            return false;
        }

        private bool GetData(Block block)
        {
            if (block == null)
                return false;

            if (block.Tag == BlockTag.Definition && 0 != (settings.Features & TableCaptionsFeatures.Definition))
            {
                block.TableCaptionData = new TableCaptionData();
                return true;
            }

            if (block.Tag != BlockTag.Paragraph || settings.Leads == null || settings.Leads.Length == 0)
                return false;

            var inline = block.InlineContent;
            if (inline == null)
                return false;

            var content = inline.LiteralContent;
            if (content == null)
                return false;
            
            var index = content.IndexOf(':');
            if (index <= 0)
                return false;

            var prefix = content.Substring(0, index - 1).TrimEnd();
            foreach (var lead in settings.Leads)
            {
                if (prefix.Equals(lead))
                {
                    inline.LiteralContent = content.Substring(index + 1).TrimStart();
                    block.TableCaptionData = new TableCaptionData
                    {
                        Lead = lead,
                    };
                    return true;
                }
            }

            return false;
        }

        private static bool IncorporateLeading(Block previous)
        {
            var caption = previous.NextSibling;
            var table = caption.NextSibling;

            Incorporate(table, caption, CaptionPlacement.Before);

#pragma warning disable 0618
            table.Previous = previous;
#pragma warning restore 0618
            if (previous != null)
                previous.NextSibling = table;

            return true;
        }

        private static bool IncorporateTrailing(Block table)
        {
            var caption = Incorporate(table, table.NextSibling, CaptionPlacement.After);
            var next = caption.NextSibling;

            table.NextSibling = next;
#pragma warning disable 0618
            if (next != null)
                next.Previous = table;
#pragma warning restore 0618

            return true;
        }

        private static Block Incorporate(Block table, Block caption, CaptionPlacement placement)
        {
            caption.Parent = table;
            caption.Tag = BlockTag.TableCaption;
            caption.TableCaptionData.Placement = placement;

            var first = table.FirstChild;
            table.FirstChild = caption;
#pragma warning disable 0618
            first.Previous = caption;
            caption.Previous = null;
#pragma warning restore 0618
            caption.NextSibling = first;

            return caption;
        }
    }
}
