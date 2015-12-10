﻿namespace CommonMark
{
    /// <summary>
    /// Contains the table settings. Used in the <see cref="CommonMarkSettings.Tables"/> property.
    /// These are only applicable if tables are enabled.
    /// </summary>
    public sealed class TableSettings
    {
        private readonly CommonMarkSettings _settings;

        internal TableSettings(CommonMarkSettings settings)
        {
            this._settings = settings;
        }

        private PipeTableFeatures _pipeTables;

        /// <summary>
        /// Gets or sets any pipe tables features that the parser and/or formatter will recognize.
        /// These are only applicable if <see cref="CommonMarkAdditionalFeatures.PipeTables"/> is enabled.
        /// </summary>
        public PipeTableFeatures PipeTables
        {
            get { return _pipeTables; }
            set
            {
                this._pipeTables = value;
                this._settings.Reset();
            }
        }

        private Syntax.TableCaptionType _captionTypes;

        /// <summary>
        /// Gets or sets any caption types that the parser and/or formatter will recognize.
        /// </summary>
        public Syntax.TableCaptionType CaptionTypes
        {
            get { return _captionTypes; }
            set
            {
                this._captionTypes = value;
                this._settings.Reset();
            }
        }
    }
}