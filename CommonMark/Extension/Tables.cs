using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Base tables extension class.
    /// </summary>
    public abstract class Tables : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tables"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected Tables(CommonMarkSettings settings)
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
                { BlockTag.TableCell, new TableCellFormatter(parameters) },
                { BlockTag.TableRow, new BlockFormatter(parameters, BlockTag.TableRow, "tr") },
                { BlockTag.TableBody, new BlockFormatter(parameters, BlockTag.TableBody, "tbody") },
                { BlockTag.TableHeader, new BlockFormatter(parameters, BlockTag.TableHeader, "thead") },
                { BlockTag.Table, new TableFormatter(parameters) },
            };
        }
    }
}
