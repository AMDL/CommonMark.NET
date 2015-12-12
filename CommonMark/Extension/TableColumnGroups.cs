using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Table column groups.
    /// </summary>
    public class TableColumnGroups : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnGroups"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public TableColumnGroups(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Creates the mapping from block tag to block element formatter.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected override IDictionary<BlockTag, IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            return new Dictionary<BlockTag, IBlockFormatter>
            {
                { BlockTag.TableColumn, new TableColumnFormatter(parameters) },
                { BlockTag.TableColumnGroup, new TableColumnGroupFormatter(parameters) },
            };
        }
    }
}
