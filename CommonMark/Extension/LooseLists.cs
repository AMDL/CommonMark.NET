using CommonMark.Formatters;

namespace CommonMark.Extension
{
    /// <summary>
    /// Loose lists settings.
    /// </summary>
    public struct LooseListsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LooseListsSettings"/> structure.
        /// </summary>
        /// <param name="isTight">
        /// <c>true</c> to render all lists tightly, or
        /// <c>false</c> to render all lists loosely.
        /// </param>
        public LooseListsSettings(bool isTight)
        {
            IsTight = isTight;
        }

        /// <summary>
        /// Gets or sets a value indicating whether list items should always be rendered tightly.
        /// </summary>
        /// <value>
        /// <c>true</c> to render all lists tightly, or
        /// <c>false</c> to render all lists loosely.
        /// </value>
        public bool IsTight { get; set; }
    }

    /// <summary>
    /// List items will always be rendered loosely/tightly.
    /// </summary>
    public sealed class LooseLists : CommonMarkExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LooseLists"/> class.
        /// </summary>
        /// <param name="settings">Loose lists settings.</param>
        public LooseLists(LooseListsSettings settings)
        {
            Settings = settings;
        }

        private LooseListsSettings Settings
        {
            get;
        }

        /// <summary>
        /// Initializes the formatting properties.
        /// </summary>
        /// <param name="parameters">Formatting parameters.</param>
        public override void InitializeFormatting(FormatterParameters parameters)
        {
            parameters.IsForceTightLists = Settings.IsTight;
        }
    }
}
