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
        /// Initializes a new instance of the <see cref="HardLineBreaks"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public HardLineBreaks(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes the inline formatters.
        /// </summary>
        protected override IEnumerable<IInlineFormatter> InitializeInlineFormatters(FormatterParameters parameters)
        {
            yield return new HardBreakFormatter(parameters);
        }
    }
}
