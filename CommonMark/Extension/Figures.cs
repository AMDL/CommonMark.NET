using CommonMark.Formatters;
using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Base figures extension class.
    /// </summary>
    public abstract class Figures : CommonMarkExtension
    {
        /// <summary>
        /// Initializes the block formatters.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        protected override IEnumerable<IBlockFormatter> InitializeBlockFormatters(FormatterParameters parameters)
        {
            yield return new CaptionFormatter(parameters, BlockTag.FigureCaption, "figcaption");
            yield return new BlockFormatter(parameters, BlockTag.Figure, htmlTags: "figure");
        }
    }
}
