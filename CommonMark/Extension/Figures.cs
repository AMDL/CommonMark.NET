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
        /// Initializes a new instance of the <see cref="Figures"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected Figures(CommonMarkSettings settings)
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
                { BlockTag.FigureCaption, new CaptionFormatter(parameters, BlockTag.FigureCaption, "figcaption") },
                { BlockTag.Figure, new BlockFormatter(parameters, BlockTag.Figure, "figure") },
            };
        }
    }
}
