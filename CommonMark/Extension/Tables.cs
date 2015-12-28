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
        /// Initializes the block formatters.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected override IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            yield return new TableBodyCellFormatter(parameters);
            yield return new BlockFormatter(parameters, BlockTag.TableHeaderCell, htmlTags: "th");
            yield return new BlockFormatter(parameters, BlockTag.TableRow, htmlTags: "tr");
            yield return new BlockFormatter(parameters, BlockTag.TableBody, htmlTags: "tbody");
            yield return new BlockFormatter(parameters, BlockTag.TableHeader, htmlTags: "thead");
            yield return new TableFormatter(parameters);
        }
    }
}
