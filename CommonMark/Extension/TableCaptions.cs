using CommonMark.Formatters;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCaptions"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tableCaptionsSettings">Table captions settings.</param>
        public TableCaptions(CommonMarkSettings settings, TableCaptionsSettings tableCaptionsSettings)
            : base(settings)
        {
            this.settings = tableCaptionsSettings;
        }

        /// <summary>
        /// Creates the mapping from block tag to block element formatter.
        /// </summary>
        protected override IDictionary<BlockTag, Formatters.IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            return new Dictionary<BlockTag, Formatters.IBlockFormatter>
            {
                { BlockTag.TableCaption, new TableCaptionFormatter(parameters) }
            };
        }

        internal bool IncorporateCaption(Block block)
        {
            return ((0 != (settings.Features & TableCaptionsFeatures.After) && block.Tag == BlockTag.Table
                        && !HasCaption(block) && Preprocess(block.NextSibling) && IncorporateTrailing(block))
                    || (0 != (settings.Features & TableCaptionsFeatures.Before) && block.NextSibling?.NextSibling?.Tag == BlockTag.Table
                        && !HasCaption(block.NextSibling.NextSibling) && Preprocess(block.NextSibling) && IncorporateLeading(block)));
        }

        private static bool HasCaption(Block table)
        {
            for (var child = table.FirstChild; child != null; child = child.NextSibling)
                if (child.Tag == BlockTag.TableCaption)
                    return true;
            return false;
        }

        private bool Preprocess(Block block)
        {
            if (block == null)
                return false;

            if (block.Tag == BlockTag.Definition)
                return PreprocessDefinition(block);

            if (block.Tag != BlockTag.Paragraph)
                return false;

            var inline = block.InlineContent;
            if (inline == null)
                return false;

            var content = inline.LiteralContent;
            if (content == null)
                return false;

            var index = content.IndexOf(':');
            if (0 != (settings.Features & TableCaptionsFeatures.ColonDefinition) && index == 0)
                return PreprocessEmptyLead(block, inline, content, index, ':');

            if (settings.Leads == null || settings.Leads.Length == 0)
                return (0 != (settings.Features & TableCaptionsFeatures.TildeDefinition) && content[0] == '~'
                    && PreprocessEmptyLead(block, inline, content, index, '~'));

            var prefix = content.Substring(0, index - 1).TrimEnd();
            foreach (var lead in settings.Leads)
                if (prefix.Equals(lead))
                    return PreprocessMatch(block, inline, content, index, prefix);

            return false;
        }

        private bool PreprocessDefinition(Block block)
        {
            if ((0 != (settings.Features & TableCaptionsFeatures.ColonDefinition) && block.ListData.BulletChar == ':')
                || (0 != (settings.Features & TableCaptionsFeatures.TildeDefinition) && block.ListData.BulletChar == '~'))
            {
                block.CaptionData = new CaptionData();
                return true;
            }
            return false;
        }

        private static bool PreprocessEmptyLead(Block block, Inline inline, string content, int index, char delimiter)
        {
            block.ListData = new ListData
            {
                BulletChar = delimiter,
            };
            return PreprocessMatch(block, inline, content, index, null);
        }

        private static bool PreprocessMatch(Block block, Inline inline, string content, int index, string prefix)
        {
            inline.LiteralContent = content.Substring(index + 1).TrimStart();
            block.CaptionData = new CaptionData
            {
                Lead = prefix,
            };
            return true;
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
            caption.CaptionData.Placement = placement;

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
