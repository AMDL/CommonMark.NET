using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark
{
    /// <summary>
    /// Prologue line handler.
    /// </summary>
    /// <param name="line">Single input line. If it terminated the prologue, it must be changed to <c>null</c>.</param>
    /// <returns><c>true</c> if the prologue is incomplete, or <c>false</c> otherwise.</returns>
    public delegate bool PrologueLineHandler(ref string line);

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
        public bool TrackSourcePosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parser handles a document prologue.
        /// This may be useful if e.g. a YAML metadata block is present.
        /// </summary>
        /// <value><c>true</c> if a document prologue should be handled, or <c>false</c> otherwise.</value>
        public bool HandlePrologue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parser includes the prologue line in the line count.
        /// </summary>
        /// <value><c>true</c> if the prologue lines should be included in the line count, or <c>false</c> otherwise.</value>
        public bool IncludePrologueInLineCount { get; set; }

        /// <summary>
        /// Gets or sets the prologue line handler.
        /// </summary>
        /// <value>
        /// Prologue line handler.
        /// Should be specified if <see cref="HandlePrologue"/> is <c>true</c>.
        /// </value>
        public PrologueLineHandler PrologueLineHandler { get; set; }

        private CommonMarkAdditionalFeatures _additionalFeatures;

        /// <summary>
        /// Gets or sets any additional features (that are not present in the current CommonMark specification) that
        /// the parser and/or formatter will recognize.
        /// </summary>
        public CommonMarkAdditionalFeatures AdditionalFeatures
        {
            get { return this._additionalFeatures; }
            set
            {
                this._additionalFeatures = value;
                this.Reset();
            }
        }

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
            clone.Reset();
            return clone;
        }

        private void Reset()
        {
            this._inlineParserParameters = new Lazy<Parser.InlineParserParameters>(GetInlineParserParameters);
            this._inlineParserEmphasisParameters = new Lazy<Parser.InlineParserParameters>(GetInlineParserEmphasisParameters);
            this._inlineSingleCharTags = new Lazy<InlineTag?[]>(GetInlineSingleCharTags);
            this._inlineDoubleCharTags = new Lazy<InlineTag?[]>(GetInlineDoubleCharTags);
        }

        #region [ Properties that cache parser parameters ]

        private Lazy<Parser.InlineParserParameters> _inlineParserParameters;

        /// <summary>
        /// Gets the parameters for parsing inline elements according to these settings.
        /// </summary>
        internal Parser.InlineParserParameters InlineParserParameters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _inlineParserParameters.Value; }
        }

        private Parser.InlineParserParameters GetInlineParserParameters()
        {
            return new Parser.InlineParserParameters(GetInlineParsers, GetInlineParserSpecialCharacters);
        }

        private Lazy<Parser.InlineParserParameters> _inlineParserEmphasisParameters;

        /// <summary>
        /// Gets the parameters for parsing inline emphasis elements.
        /// </summary>
        internal Parser.InlineParserParameters InlineParserEmphasisParameters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _inlineParserEmphasisParameters.Value; }
        }

        private Parser.InlineParserParameters GetInlineParserEmphasisParameters()
        {
            return new Parser.InlineParserParameters(GetInlineEmphasisParsers, GetInlineParserEmphasisSpecialCharacters);
        }

        private Func<Parser.Subject, Syntax.Inline>[] InlineParsers
        {
            get { return InlineParserParameters.Parsers; }
        }

        private Func<Parser.Subject, Syntax.Inline>[] GetInlineParsers()
        {
            return Parser.InlineMethods.InitializeParsers(this);
        }

        private Func<Parser.Subject, Syntax.Inline>[] InlineEmphasisParsers
        {
            get { return InlineParserEmphasisParameters.Parsers; }
        }

        private Func<Parser.Subject, Syntax.Inline>[] GetInlineEmphasisParsers()
        {
            return Parser.InlineMethods.InitializeEmphasisParsers(this);
        }

        private char[] GetInlineParserSpecialCharacters()
        {
            var p = this.InlineParsers;
            var vs = new List<char>(20);
            for (var i = 0; i < p.Length; i++)
                if (p[i] != null)
                    vs.Add((char)i);

            return vs.ToArray();
        }

        private char[] GetInlineParserEmphasisSpecialCharacters()
        {
            var p = this.InlineEmphasisParsers;
            var vs = new List<char>(2);
            for (var i = 0; i < p.Length; i++)
                if (p[i] != null)
                    vs.Add((char)i);

            return vs.ToArray();
        }

        private Lazy<InlineTag?[]> _inlineSingleCharTags;
        internal InlineTag?[] InlineSingleCharTags
        {
            get { return _inlineSingleCharTags.Value; }
        }

        private InlineTag?[] GetInlineSingleCharTags()
        {
            return Parser.InlineMethods.InitializeSingleCharTags(this);
        }

        private Lazy<InlineTag?[]> _inlineDoubleCharTags;
        internal InlineTag?[] InlineDoubleCharTags
        {
            get { return _inlineDoubleCharTags.Value; }
        }

        private InlineTag?[] GetInlineDoubleCharTags()
        {
            return Parser.InlineMethods.InitializeDoubleCharTags(this);
        }

        #endregion
    }
}
