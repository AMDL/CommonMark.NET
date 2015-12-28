using CommonMark.Formatters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Definition lists.
    /// </summary>
    public class DefinitionLists : CommonMarkExtension
    {
        private readonly DefinitionListsSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionLists"/> class.
        /// </summary>
        /// <param name="definitionListsSettings">Definition lists settings.</param>
        public DefinitionLists(DefinitionListsSettings definitionListsSettings)
        {
            settings = definitionListsSettings;
        }

        /// <summary>
        /// Initializes the block formatters.
        /// </summary>
        protected override IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            yield return new BlockFormatter(parameters, BlockTag.DefinitionList, "dl");
            yield return new BlockFormatter(parameters, BlockTag.Term, "dt");
            yield return new BlockFormatter(parameters, BlockTag.Definition, "dd");
        }
    }
}
