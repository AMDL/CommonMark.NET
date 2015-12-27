using System;

namespace CommonMark
{
    /// <summary>
    /// Specifies different formatters supported by the converter.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// The output is standard HTML format according to the CommonMark specification.
        /// </summary>
        Html,

        /// <summary>
        /// The output is a text view of the syntax tree. Usable for debugging.
        /// </summary>
        TextSyntaxTree,

        /// <summary>
        /// The output is a text view of the syntax tree. Usable for debugging.
        /// </summary>
        [Obsolete("Use " + nameof(TextSyntaxTree) + "() instead.")]
        SyntaxTree = TextSyntaxTree,

        /// <summary>
        /// The output is written using a delegate function specified in <see cref="CommonMarkSettings.OutputDelegate"/>.
        /// </summary>
        [Obsolete("Use " + nameof(CommonMarkConverter.Parse) + "() instead.")]
        CustomDelegate
    }
}
