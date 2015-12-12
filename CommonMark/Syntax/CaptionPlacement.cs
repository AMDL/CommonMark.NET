namespace CommonMark.Syntax
{
    /// <summary>
    /// Caption placement.
    /// </summary>
    public enum CaptionPlacement
    {
        /// <summary>
        /// The caption element immediately precedes the container element.
        /// </summary>
        Before = 1, // to match TableCaptionsFeatures

        /// <summary>
        /// The caption element immediately follows the container element.
        /// </summary>
        After = 2, // to match TableCaptionsFeatures
    }
}
