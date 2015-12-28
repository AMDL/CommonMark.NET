using CommonMark.Parser;

namespace CommonMark.Extension
{
    /// <summary>
    /// Reference label case.
    /// </summary>
    public enum ReferenceCaseType
    {
        /// <summary>
        /// The label case will be preserved. This is the default.
        /// </summary>
        None = 0,

        /// <summary>
        /// Labels will be converted to upper case using the invariant culture.
        /// This is similar to how the parser behaves without the extension being registered.
        /// </summary>
        UpperInvariant = 1,

        /// <summary>
        /// Labels will be converted to lower case using the invariant culture.
        /// </summary>
        LowerInvariant = 2,

        /// <summary>
        /// Labels will be converted to upper case using the current culture.
        /// </summary>
        Upper = 5,

        /// <summary>
        /// Labels will be converted to lower case using the current culture.
        /// </summary>
        Lower = 6,
    }

    /// <summary>
    /// Reference label case settings.
    /// </summary>
    public struct ReferenceCaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCaseSettings"/> structure.
        /// </summary>
        /// <param name="case">Label case.</param>
        public ReferenceCaseSettings(ReferenceCaseType @case)
        {
            Case = @case;
        }

        /// <summary>
        /// Gets or sets the label case.
        /// </summary>
        public ReferenceCaseType Case { get; set; }
    }

    /// <summary>
    /// Configurable reference label case.
    /// </summary>
    public class ReferenceCase : CommonMarkExtension
    {
        private readonly ReferenceCaseSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCase"/> class.
        /// </summary>
        /// <param name="referenceCaseSettings">Reference case settings.</param>
        public ReferenceCase(ReferenceCaseSettings referenceCaseSettings)
        {
            settings = referenceCaseSettings;
        }

        /// <summary>
        /// Initializes the inline parsing properties.
        /// </summary>
        /// <param name="parameters">Inline parser parameters.</param>
        public override void InitializeInlineParsing(InlineParserParameters parameters)
        {
            parameters.CompleteNormalizeReference = NormalizeReference;
        }

        private string NormalizeReference(string s)
        {
            switch (settings.Case)
            {
                case ReferenceCaseType.UpperInvariant:
                    return s.ToUpperInvariant();
                case ReferenceCaseType.LowerInvariant:
                    return s.ToLowerInvariant();
                case ReferenceCaseType.Upper:
                    return s.ToUpper();
                case ReferenceCaseType.Lower:
                    return s.ToLower();
                case ReferenceCaseType.None:
                default:
                    return s;
            }
        }
    }
}
