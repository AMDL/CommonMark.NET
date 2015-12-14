﻿using System;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Class used to configure the behavior of <see cref="CommonMarkConverter"/>.
    /// </summary>
    /// <remarks>This class is not thread-safe so any changes to a instance that is reused (for example, the 
    /// <see cref="CommonMarkSettings.Default"/>) has to be updated while it is not in use otherwise the
    /// behaviour is undefined.</remarks>
    public sealed class CommonMarkSettings
    {
        /// <summary>Initializes a new instance of the <see cref="CommonMarkSettings" /> class.</summary>
        [Obsolete("Use CommonMarkSettings.Default.Clone() instead", false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public CommonMarkSettings()
        {
            this._extensions = new CommonMarkExtensionCollection(this);
            Reset();
        }

        /// <summary>
        /// Gets or sets the output format used by the last stage of conversion.
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        private Action<Syntax.Block, System.IO.TextWriter, CommonMarkSettings> _outputDelegate;
        /// <summary>
        /// Gets or sets the custom output delegate function used for formatting CommonMark output.
        /// Setting this to a non-null value will also set <see cref="OutputFormat"/> to <see cref="OutputFormat.CustomDelegate"/>.
        /// </summary>
        public Action<Syntax.Block, System.IO.TextWriter, CommonMarkSettings> OutputDelegate
        {
            get { return this._outputDelegate; }
            set
            {
                if (this._outputDelegate != value)
                {
                    this._outputDelegate = value;
                    this.OutputFormat = value == null ? default(OutputFormat) : OutputFormat.CustomDelegate;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether soft line breaks should be rendered as hard line breaks.
        /// </summary>
        public bool RenderSoftLineBreaksAsLineBreaks { get; set; }

        private bool _trackSourcePosition;

        /// <summary>
        /// Gets or sets a value indicating whether the parser tracks precise positions in the source data for
        /// block and inline elements. This is disabled by default because it incurs an additional performance cost to
        /// keep track of the original position.
        /// Setting this to <c>true</c> will populate <see cref="Syntax.Inline.SourcePosition"/>, 
        /// <see cref="Syntax.Inline.SourceLength"/>, <see cref="Syntax.Block.SourcePosition"/> and 
        /// <see cref="Syntax.Block.SourceLength"/> properties with correct information, otherwise the values
        /// of these properties are undefined.
        /// This also controls if these values will be written to the output.
        /// </summary>
        public bool TrackSourcePosition
        {
            get { return _trackSourcePosition; }
            set { _trackSourcePosition = value; }
        }

        #region Extensions

#pragma warning disable 0618
        private CommonMarkAdditionalFeatures _additionalFeatures;
#pragma warning restore 0618

        /// <summary>
        /// Gets or sets any additional features (that are not present in the current CommonMark specification) that
        /// the parser and/or formatter will recognize.
        /// </summary>
        [Obsolete("Use " + nameof(CommonMarkSettings.Extensions) + " instead.")]
        public CommonMarkAdditionalFeatures AdditionalFeatures
        {
            get { return this._additionalFeatures; }
            set
            {
                this._additionalFeatures = value;
                var strikeout = new Extension.Strikeout(this);
                var strikethroughTilde = 0 != (value & CommonMarkAdditionalFeatures.StrikethroughTilde);
                if (strikethroughTilde && !Extensions.IsRegistered(strikeout))
                    Extensions.Register(strikeout);
                if (!strikethroughTilde && Extensions.IsRegistered(strikeout))
                    Extensions.Unregister(strikeout);
                this.Reset();
            }
        }

        private CommonMarkExtensionCollection _extensions;

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        public CommonMarkExtensionCollection Extensions
        {
            get { return _extensions; }
        }

        #endregion Extensions

        private Func<string, string> _uriResolver;
        /// <summary>
        /// Gets or sets the delegate that is used to resolve addresses during rendering process. Can be used to process application relative URLs (<c>~/foo/bar</c>).
        /// </summary>
        /// <example><code>CommonMarkSettings.Default.UriResolver = VirtualPathUtility.ToAbsolute;</code></example>
        public Func<string, string> UriResolver
        {
            get { return this._uriResolver; }
            set
            {
                if (value != null)
                {
                    var orig = value;
                    value = x =>
                    {
                        try
                        {
                            return orig(x);
                        }
                        catch (Exception ex)
                        {
                            throw new CommonMarkException("An error occurred while executing the CommonMarkSettings.UriResolver delegate. View inner exception for details.", ex);
                        }
                    };
                }

                this._uriResolver = value;
            }
        }

#pragma warning disable 0618
        private static readonly CommonMarkSettings _default = new CommonMarkSettings();
#pragma warning restore 0618

        /// <summary>
        /// The default settings for the converter. If the properties of this instance are modified, the changes will be applied to all
        /// future conversions that do not specify their own settings.
        /// </summary>
        public static CommonMarkSettings Default { get { return _default; } }

        /// <summary>
        /// Creates a copy of this configuration object.
        /// </summary>
        public CommonMarkSettings Clone()
        {
            var clone = (CommonMarkSettings)this.MemberwiseClone();
            clone._extensions = new CommonMarkExtensionCollection(clone, this.Extensions);
            clone.Reset();
            return clone;
        }

        internal void Reset()
        {
            this._blockParserParameters = GetLazy(GetBlockParserParameters);
            this._inlineParserParameters = GetLazy(GetInlineParserParameters);
            this._formatterParameters = GetLazy(GetFormatterParameters);
        }

        internal Lazy<T> GetLazy<T>(Func<T> valueFactory)
        {
            return new Lazy<T>(valueFactory, isThreadSafe: false);
        }

        #region BlockParserParameters

        private Lazy<Parser.BlockParserParameters> _blockParserParameters;

        /// <summary>
        /// Gets the block element parser parameters.
        /// </summary>
        internal Parser.BlockParserParameters BlockParserParameters
        {
            get { return _blockParserParameters.Value; }
        }

        private Parser.BlockParserParameters GetBlockParserParameters()
        {
            return new Parser.BlockParserParameters(this);
        }

        #endregion BlockParserParameters

        #region InlineParserParameters

        private Lazy<Parser.StandardInlineParserParameters> _inlineParserParameters;

        /// <summary>
        /// Gets the parameters for parsing inline elements according to these settings.
        /// </summary>
        public Parser.InlineParserParameters InlineParserParameters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _inlineParserParameters.Value; }
        }

        private Parser.StandardInlineParserParameters GetInlineParserParameters()
        {
            return new Parser.StandardInlineParserParameters(this);
        }

        #endregion InlineParserParameters

        #region FormatterParameters

        private Lazy<Formatters.FormatterParameters> _formatterParameters;

        /// <summary>
        /// Gets the formatter parameters.
        /// </summary>
        public Formatters.FormatterParameters FormatterParameters
        {
            get { return _formatterParameters.Value; }
        }

        private Formatters.FormatterParameters GetFormatterParameters()
        {
            return new Formatters.FormatterParameters(this);
        }

        #endregion FormatterParameters
    }
}
