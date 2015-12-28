using System.Collections.Generic;
using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;

namespace CommonMark.Extension
{
    /// <summary>
    /// Render soft line breaks as hard line breaks.
    /// </summary>
    public sealed class HardLineBreaks : CommonMarkExtension
    {
        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new HardBreakFormatter(parameters);
        }
    }
}
