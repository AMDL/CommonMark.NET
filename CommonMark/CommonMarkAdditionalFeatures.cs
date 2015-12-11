using System;

namespace CommonMark
{
    /// <summary>
    /// Lists additional features that can be enabled in <see cref="CommonMarkSettings"/>.
    /// These features are not part of the standard and should not be used if interoperability with other
    /// CommonMark implementations is required.
    /// </summary>
    [Flags]
    public enum CommonMarkAdditionalFeatures
    {
        /// <summary>
        /// No additional features are enabled. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// The parser will recognize syntax <c>~~foo~~</c> that will be rendered as <c>&lt;del&gt;foo&lt;/del&gt;</c>.
        /// </summary>
        [Obsolete("Use " + nameof(Extension.Strikeout) + " instead.")]
        StrikethroughTilde = 1,

        /// <summary>
        /// The parser will recognize pipe tables.
        /// </summary>
        PipeTables = 1024,

        /// <summary>
        /// All additional features are enabled.
        /// </summary>
        All = 0x7FFFFFFF
    }
}
