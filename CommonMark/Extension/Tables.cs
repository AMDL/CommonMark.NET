using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Base class for all tables extensions.
    /// </summary>
    public abstract class Tables : CommonMarkExtension
    {
        private readonly Lazy<Dictionary<BlockTag, IBlockFormatter>> _formatters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tables"/> class.
        /// </summary>
        /// <param name="settings">CommonMark settings.</param>
        protected Tables(CommonMarkSettings settings)
        {
            this._formatters = new Lazy<Dictionary<BlockTag, IBlockFormatter>>(() => InitializeFormatters(settings.FormatterParameters));
        }

        /// <summary>
        /// Gets the mapping from block tag to block element formatter.
        /// </summary>
        public override IDictionary<BlockTag, IBlockFormatter> BlockFormatters
        {
            get { return _formatters.Value; }
        }

        /// <summary>
        /// Creates the mapping from block tag to block element formatter.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected virtual Dictionary<BlockTag, IBlockFormatter> InitializeFormatters(FormatterParameters parameters)
        {
            return new Dictionary<BlockTag, IBlockFormatter>
            {
                { BlockTag.TableCell, new TableCellFormatter(parameters) },
                { BlockTag.TableRow, new TableRowFormatter(parameters) },
                { BlockTag.TableBody, new TableBodyFormatter(parameters) },
                { BlockTag.TableFooter, new TableFooterFormatter(parameters) },
                { BlockTag.TableHeader, new TableHeaderFormatter(parameters) },
                { BlockTag.TableColumn, new TableColumnFormatter(parameters) },
                { BlockTag.TableColumnGroup, new TableColumnGroupFormatter(parameters) },
                { BlockTag.Table, new TableFormatter(parameters) },
            };
        }
    }
}
