using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Definition lists.
    /// </summary>
    public class DefinitionLists : CommonMarkExtension
    {
        private readonly DefinitionListsSettings settings;
        private readonly Lazy<Dictionary<BlockTag, IBlockFormatter>> _formatters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionLists"/> class.
        /// </summary>
        /// <param name="settings">The object containing settings for the formatting process.</param>
        /// <param name="definitionListsSettings">Definition lists settings.</param>
        public DefinitionLists(CommonMarkSettings settings, DefinitionListsSettings definitionListsSettings)
        {
            this.settings = definitionListsSettings;
            this._formatters = new Lazy<Dictionary<BlockTag, IBlockFormatter>>(() => InitializeFormatters(settings.FormatterParameters));
        }

        /// <summary>
        /// Gets the mapping from block tag to block element formatter.
        /// </summary>
        public override IDictionary<BlockTag, IBlockFormatter> BlockFormatters
        {
            get { return _formatters.Value; }
        }

        private Dictionary<BlockTag, IBlockFormatter> InitializeFormatters(FormatterParameters parameters)
        {
            return new Dictionary<BlockTag, IBlockFormatter>
            {
                { BlockTag.DefinitionList, new DefinitionListFormatter(parameters) },
                { BlockTag.Term, new TermFormatter(parameters) },
                { BlockTag.Definition, new DefinitionFormatter(parameters) },
            };
        }
    }
}
