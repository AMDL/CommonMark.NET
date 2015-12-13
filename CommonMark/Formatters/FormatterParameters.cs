using System;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Formatter parameters.
    /// </summary>
    public struct FormatterParameters
    {
        /// <summary>
        /// Gets or sets a value indicating whether source positions need to be written to the output.
        /// </summary>
        public bool TrackPositions { get; set; }

        /// <summary>
        /// Gets or sets the delegate that is used to resolve addresses during rendering process.
        /// </summary>
        public Func<string, string> UriResolver { get; set; }
    }
}
