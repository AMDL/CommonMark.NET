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
        /// Initializes the block formatters.
        /// </summary>
        protected override IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            yield return new CaptionFormatter(parameters, BlockTag.TableCaption, "caption");
        }

        internal bool IncorporateCaption(Block block)
        {
            return ((IsEnabled(TableCaptionsFeatures.After) && block.Tag == BlockTag.Table
                        && !HasCaption(block) && Preprocess(block.NextSibling) && IncorporateTrailing(block))
                    || (IsEnabled(TableCaptionsFeatures.Before) && block.NextSibling?.NextSibling?.Tag == BlockTag.Table
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
            if (content == null || content.Length <= 1)
                return false;

            if ((IsEnabled(TableCaptionsFeatures.ColonDefinition) && PreprocessEmptyLead(block, inline, content, ':'))
                || (IsEnabled(TableCaptionsFeatures.TildeDefinition) && PreprocessEmptyLead(block, inline, content, '~')))
                return true;

            if (settings.Leads == null || settings.Leads.Length == 0)
                return false;
            var index = content.IndexOf(':');
            var prefix = content.Substring(0, index);
            if (IsEnabled(TableCaptionsFeatures.TrimLead))
                prefix = prefix.TrimEnd();

            foreach (var lead in settings.Leads)
                if (prefix.Equals(lead))
                    return PreprocessMatch(block, inline, content, index, prefix);

            return false;
        }

        private bool PreprocessDefinition(Block block)
        {
            if ((IsEnabled(TableCaptionsFeatures.ColonDefinition) && block.ListData.BulletChar == ':')
                || (IsEnabled(TableCaptionsFeatures.TildeDefinition) && block.ListData.BulletChar == '~'))
            {
                block.Caption = new CaptionData();
                return true;
            }
            return false;
        }

        private static bool PreprocessEmptyLead(Block block, Inline inline, string content, char delimiter)
        {
            if (content[0] != delimiter || !Utilities.IsWhitespace(content[1]))
                return false;
            block.ListData = new ListData
            {
                BulletChar = delimiter,
            };
            return PreprocessMatch(block, inline, content, 1, null);
        }

        private static bool PreprocessMatch(Block block, Inline inline, string content, int index, string prefix)
        {
            inline.LiteralContent = content.Substring(index + 1).TrimStart();
            block.Caption = new CaptionData
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
            caption.Caption.Placement = placement;

            var first = table.FirstChild;
            table.FirstChild = caption;
#pragma warning disable 0618
            first.Previous = caption;
            caption.Previous = null;
#pragma warning restore 0618
            caption.NextSibling = first;

            return caption;
        }

        private bool IsEnabled(TableCaptionsFeatures feature)
        {
            return 0 != (settings.Features & feature);
        }
    }
}
