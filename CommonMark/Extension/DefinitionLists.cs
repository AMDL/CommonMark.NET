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
        /// <param name="settings">Common settings.</param>
        /// <param name="definitionListsSettings">Definition lists settings.</param>
        public DefinitionLists(CommonMarkSettings settings, DefinitionListsSettings definitionListsSettings)
            : base(settings)
        {
            this.settings = definitionListsSettings;
        }

        /// <summary>
        /// Creates the mapping from block tag to block element formatter.
        /// </summary>
        protected override IDictionary<BlockTag, IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            return new Dictionary<BlockTag, IBlockFormatter>
            {
                { BlockTag.DefinitionList, new BlockFormatter(parameters, BlockTag.DefinitionList, "dl") },
                { BlockTag.Term, new BlockFormatter(parameters, BlockTag.Term, "dt") },
                { BlockTag.Definition, new BlockFormatter(parameters, BlockTag.Definition, "dd") },
            };
        }
    }
}
