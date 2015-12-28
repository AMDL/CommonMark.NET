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
        /// Initializes the block parsers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected override IEnumerable<IBlockParser> InitializeBlockParsers(CommonMarkSettings settings)
        {
#pragma warning disable 0618
            yield return new ListParser(settings, BlockTag.List, BlockTag.ListItem);

            var unorderedParameters = UnorderedListItemHandler.DefaultParameters.Clone();
            unorderedParameters.ParentTag = BlockTag.List;
            unorderedParameters.Delimiters = InitializeDelimiters(unorderedParameters.Delimiters);
            var numericParameters = NumericListItemHandler.DefaultParameters.Clone();
            numericParameters.ParentTag = BlockTag.List;
            var listParameters = new ListParameters(unorderedParameters, numericParameters);

            yield return new ListItemParser(settings, BlockTag.ListItem, listParameters);
#pragma warning restore 0618
        }

        /// <summary>
        /// Initializes the escapable characters.
        /// </summary>
        protected override IEnumerable<char> InitializeEscapableCharacters()
        {
            yield return '•';
        }

        /// <summary>
        /// Initializes the block formatters.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected override IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            yield return new LegacyUnorderedListFormatter(parameters);
            yield return new LegacyOrderedListFormatter(parameters);
        }

        private UnorderedListItemDelimiterParameters[] InitializeDelimiters(UnorderedListItemDelimiterParameters[] delimiters)
        {
            var list = new List<UnorderedListItemDelimiterParameters>(delimiters);
            list.Add(new UnorderedListItemDelimiterParameters('•'));
            return list.ToArray();
        }
    }
}
