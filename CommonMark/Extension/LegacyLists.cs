using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Parser;
using CommonMark.Parser.Blocks;
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
            yield return new BulletListItemParser(settings, BlockTag.List, BulletListItemParser.DefaultParameters);
            yield return new OrderedListItemParser(settings, BlockTag.List, OrderedListItemParser.DefaultParameters);
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
