using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Parser;
using CommonMark.Parser.Blocks;
using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// CommonMark.NET backward-compatible lists.
    /// </summary>
    public class LegacyLists : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyLists"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public LegacyLists(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes the block parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected override IEnumerable<IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
#pragma warning disable 0618
            yield return new ListParser(settings, BlockTag.List, BlockTag.ListItem);

            // These will ruin the default parameters, but that's alright since we're replacing the parser anyway.
            var bulletListItemParameters = BulletListItemHandler.DefaultParameters;
            bulletListItemParameters.ParentTag = BlockTag.List;
            var orderedListItemParameters = OrderedListItemHandler.DefaultParameters;
            orderedListItemParameters.ParentTag = BlockTag.List;
            var listParameters = new ListParameters(bulletListItemParameters, orderedListItemParameters);

            yield return new ListItemParser(settings, BlockTag.ListItem, listParameters);
#pragma warning restore 0618
        }

        /// <summary>
        /// Initializes the block formatters.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected override IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            yield return new LegacyBulletListFormatter(parameters);
            yield return new LegacyOrderedListFormatter(parameters);
        }
    }
}
