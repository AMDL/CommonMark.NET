using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Base table footers extension class.
    /// </summary>
    public abstract class TableFooters : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableFooters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected TableFooters(CommonMarkSettings settings)
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
                { BlockTag.TableFooter, new TableFooterFormatter(parameters) },
            };
        }
    }
}
