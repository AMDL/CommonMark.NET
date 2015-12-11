using System;
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
            this._extensions = new List<IExtension>();
            this._tables = new Lazy<TableSettings>(GetTables);
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

        #region Extensions

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

        private List<IExtension> _extensions;

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        internal IEnumerable<IExtension> Extensions
        {
            get { return _extensions; }
        }

        /// <summary>
        /// Registers an extension. Extensions must not retain references to the settings object.
        /// </summary>
        /// <param name="extension">The extension to register.</param>
        public void Register(IExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            _extensions.Add(extension);
            this.Reset();
        }

        /// <summary>
        /// Registers multiple extensions. Extensions must not retain references to the settings object.
        /// </summary>
        /// <param name="extensions">The extensions to register.</param>
        public void Register(IEnumerable<IExtension> extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException(nameof(extensions));
            _extensions.AddRange(extensions);
            this.Reset();
        }

        /// <summary>
        /// Unregisters an extension.
        /// </summary>
        /// <param name="extension">The extension to unregister.</param>
        public void Unregister(IExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            _extensions.Remove(extension);
            this.Reset();
        }

        /// <summary>
        /// Unregisters all extensions.
        /// </summary>
        public void UnregisterAll()
        {
            _extensions.Clear();
            this.Reset();
        }

        /// <summary>
        /// Determines whether an extension is registered.
        /// </summary>
        /// <param name="extension">The extension to locate.</param>
        /// <returns><c>true</c> if the extension is registered.</returns>
        public bool IsRegistered(IExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            return _extensions.Contains(extension);
        }

        private Lazy<TableSettings> _tables;

        /// <summary>
        /// Gets the table settings. These are only applicable if tables are enabled.
        /// </summary>
        public TableSettings Tables
        {
            get { return this._tables.Value; }
        }

        private TableSettings GetTables()
        {
            return new TableSettings(this);
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
            clone._extensions = new List<IExtension>(this._extensions);
            clone._tables = new Lazy<TableSettings>(clone.GetTables);
            clone.Reset();
            return clone;
        }

        internal void Reset()
        {
            this._inlineParsers = new Lazy<InlineParserDelegate[]>(GetInlineParsers);
            this._inlineParserSpecialCharacters = new Lazy<char[]>(GetInlineParserSpecialCharacters);
            this._blockParserParameters = new Lazy<Parser.BlockParserParameters>(GetBlockParserParameters);
            this._tableParser = new Lazy<Parser.PipeTableParser>(GetTableParser);
            this._inlineSingleCharTags = new Lazy<Syntax.InlineTag[]>(GetInlineSingleCharTags);
            this._inlineDoubleCharTags = new Lazy<Syntax.InlineTag[]>(GetInlineDoubleCharTags);
            this._blockFormatters = new Lazy<Formatters.IBlockFormatter[]>(GetBlockFormatters);
            this._inlineFormatters = new Lazy<Formatters.IInlineFormatter[]>(GetInlineFormatters);
        }

        #region [ Properties that cache structures used in the parsers ]

        private Lazy<InlineParserDelegate[]> _inlineParsers;

        /// <summary>
        /// Gets the delegates that parse inline elements according to these settings.
        /// </summary>
        internal InlineParserDelegate[] InlineParsers
        {
            get { return _inlineParsers.Value; }
        }

        private InlineParserDelegate[] GetInlineParsers()
        {
            var parsers = Parser.InlineMethods.InitializeParsers(this);
            foreach (var extension in Extensions)
            {
                if (extension.InlineParsers != null)
                {
                    foreach (var kvp in extension.InlineParsers)
                    {
                        var value = kvp.Value;
                        if (value == null)
                            throw new ArgumentException("Parser delegate value cannot be null.");
                        var key = kvp.Key;
                        var inner = parsers[key];
                        if (inner != null)
                            value = new Parser.DelegateInlineParser(inner, value).ParseInline;
                        parsers[key] = value;
                    }
                }
            }
            return parsers;
        }

        private Lazy<char[]> _inlineParserSpecialCharacters;

        /// <summary>
        /// Gets the characters that have special meaning for inline element parsers according to these settings.
        /// </summary>
        internal char[] InlineParserSpecialCharacters
        {
            get { return _inlineParserSpecialCharacters.Value; }
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

        private Lazy<Parser.PipeTableParser> _tableParser;

        /// <summary>
        /// Gets the pipe table parser.
        /// </summary>
        internal Parser.PipeTableParser PipeTableParser
        {
            get { return _tableParser.Value; }
        }

        private Parser.PipeTableParser GetTableParser()
        {
            return new Parser.PipeTableParser(this);
        }

        private Lazy<Syntax.InlineTag[]> _inlineSingleCharTags;
        internal Syntax.InlineTag[] InlineSingleCharTags
        {
            get { return _inlineSingleCharTags.Value; }
        }

        private Syntax.InlineTag[] GetInlineSingleCharTags()
        {
            var tags = Parser.InlineMethods.InitializeSingleCharTags(this);
            foreach (var extension in Extensions)
            {
                if (extension.SingleCharTags != null)
                {
                    foreach (var kvp in extension.SingleCharTags)
                    {
                        var value = kvp.Value;
                        if (value == 0)
                            throw new ArgumentException("Single character tag value cannot be 0.");
                        var key = kvp.Key;
                        if (tags[key] != 0)
                            throw new InvalidOperationException("Single character tag value is already set.");
                        tags[key] = kvp.Value;
                    }
                }
            }
            return tags;
        }

        private Lazy<Syntax.InlineTag[]> _inlineDoubleCharTags;
        internal Syntax.InlineTag[] InlineDoubleCharTags
        {
            get { return _inlineDoubleCharTags.Value; }
        }

        private Syntax.InlineTag[] GetInlineDoubleCharTags()
        {
            var tags = Parser.InlineMethods.InitializeDoubleCharTags(this);
            foreach (var extension in Extensions)
            {
                if (extension.DoubleCharTags != null)
                {
                    foreach (var kvp in extension.DoubleCharTags)
                    {
                        var value = kvp.Value;
                        if (value == 0)
                            throw new ArgumentException("Double character tag value cannot be 0.");
                        var key = kvp.Key;
                        if (tags[key] != 0)
                            throw new InvalidOperationException("Double character tag value is already set.");
                        tags[key] = kvp.Value;
                    }
                }
            }
            return tags;
        }

        #endregion

        #region [ Properties that cache structures used in the formatters ]

        private Lazy<Formatters.IBlockFormatter[]> _blockFormatters;

        /// <summary>
        /// Gets the block element formatters.
        /// </summary>
        internal Formatters.IBlockFormatter[] BlockFormatters
        {
            get { return _blockFormatters.Value; }
        }

        private Formatters.IBlockFormatter[] GetBlockFormatters()
        {
            var formatters = Formatters.BlockFormatter.InitializeFormatters(this);
            foreach (var extension in Extensions)
            {
                if (extension.BlockFormatters != null)
                {
                    foreach (var kvp in extension.BlockFormatters)
                    {
                        var value = kvp.Value;
                        if (value == null)
                            throw new ArgumentException("Block formatter value cannot be null.");
                        var key = (int)kvp.Key;
                        var inner = formatters[key];
                        if (inner != null)
                            value = new Formatters.Blocks.DelegateBlockFormatter(inner, value);
                        formatters[key] = value;
                    }
                }
            }
            return formatters;
        }

        private Lazy<Formatters.IInlineFormatter[]> _inlineFormatters;

        /// <summary>
        /// Gets the inline element formatters.
        /// </summary>
        internal Formatters.IInlineFormatter[] InlineFormatters
        {
            get { return _inlineFormatters.Value; }
        }

        private Formatters.IInlineFormatter[] GetInlineFormatters()
        {
            var formatters = Formatters.InlineFormatter.InitializeFormatters(this);
            foreach (var extension in Extensions)
            {
                if (extension.InlineFormatters != null)
                {
                    foreach (var kvp in extension.InlineFormatters)
                    {
                        var value = kvp.Value;
                        if (value == null)
                            throw new ArgumentException("Inline formatter value cannot be null.");
                        var key = (int)kvp.Key;
                        var inner = formatters[key];
                        if (inner != null)
                            value = new Formatters.Inlines.DelegateInlineFormatter(inner, value);
                        formatters[key] = value;
                    }
                }
            }
            return formatters;
        }

        #endregion

    }
}
