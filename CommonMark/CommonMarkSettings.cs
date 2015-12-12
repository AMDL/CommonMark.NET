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
            this._formatterParameters = new Formatters.FormatterParameters();
            this._extensions = new Lazy<List<ICommonMarkExtension>>(() => new List<ICommonMarkExtension>());
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
            set
            {
                _trackSourcePosition = value;
                this._formatterParameters.TrackPositions = value;
            }
        }

        #region Extensions

#pragma warning disable 0618
        private CommonMarkAdditionalFeatures _additionalFeatures;
#pragma warning restore 0618

        /// <summary>
        /// Gets or sets any additional features (that are not present in the current CommonMark specification) that
        /// the parser and/or formatter will recognize.
        /// </summary>
        [Obsolete("Use " + nameof(CommonMarkSettings.Register) + "() and " + nameof(CommonMarkSettings.Unregister) + "() instead.")]
        public CommonMarkAdditionalFeatures AdditionalFeatures
        {
            get { return this._additionalFeatures; }
            set
            {
                this._additionalFeatures = value;
                var strikeout = new Extension.Strikeout(this);
                var strikethroughTilde = 0 != (value & CommonMarkAdditionalFeatures.StrikethroughTilde);
                if (strikethroughTilde && !IsRegistered(strikeout))
                    Extensions.Add(strikeout);
                if (!strikethroughTilde && IsRegistered(strikeout))
                    Extensions.Remove(strikeout);
                this.Reset();
            }
        }

        private Lazy<List<ICommonMarkExtension>> _extensions;

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        private List<ICommonMarkExtension> Extensions
        {
            get { return _extensions.Value; }
        }

        /// <summary>
        /// Registers an extension. Extensions must not retain references to the settings object.
        /// </summary>
        /// <param name="extension">The extension to register.</param>
        public void Register(ICommonMarkExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            if (Extensions.Contains(extension))
            {
                var message = string.Format("Extension is already registered: {0}.", extension.ToString());
                throw new InvalidOperationException(message);
            }

            Extensions.Add(extension);
            this.Reset();
        }

        /// <summary>
        /// Registers multiple extensions. Extensions must not retain references to the settings object.
        /// </summary>
        /// <param name="extensions">The extensions to register.</param>
        public void Register(IEnumerable<ICommonMarkExtension> extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException(nameof(extensions));

            foreach (var extension in extensions)
            {
                if (Extensions.Contains(extension))
                {
                    var message = string.Format("Extension is already registered: {0}.", extension.ToString());
                    throw new InvalidOperationException(message);
                }
            }

            Extensions.AddRange(extensions);
            this.Reset();
        }

        /// <summary>
        /// Registers all built-in extensions with all their features enabled.
        /// This may be useful in benchmarking.
        /// </summary>
        public void RegisterAll()
        {
            Register(new Extension.Strikeout(this));
            Register(new Extension.DefinitionLists(this, new Extension.DefinitionListsSettings(Extension.DefinitionListsFeatures.All)));
            Register(new Extension.PipeTables(this, new Extension.PipeTablesSettings(Extension.PipeTablesFeatures.All)));
            Register(new Extension.TableColumnGroups(this));
            Register(new Extension.TableCaptions(this, new Extension.TableCaptionsSettings
            {
                Features = Extension.TableCaptionsFeatures.All,
                Leads = new[] { "Table" },
            }));
            this.Reset();
        }

        /// <summary>
        /// Unregisters an extension.
        /// </summary>
        /// <param name="extension">The extension to unregister.</param>
        public void Unregister(ICommonMarkExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            if (!Extensions.Remove(extension))
            {
                var message = string.Format("Extension is not registered: {0}.", extension.ToString());
                throw new InvalidOperationException(message);
            }

            this.Reset();
        }

        /// <summary>
        /// Unregisters all extensions.
        /// </summary>
        public void UnregisterAll()
        {
            Extensions.Clear();
            this.Reset();
        }

        /// <summary>
        /// Determines whether an extension is registered.
        /// </summary>
        /// <param name="extension">The extension to locate.</param>
        /// <returns><c>true</c> if the extension is registered.</returns>
        public bool IsRegistered(ICommonMarkExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            return Extensions.Contains(extension);
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
                this._formatterParameters.UriResolver = value;
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
            clone._extensions = new Lazy<List<ICommonMarkExtension>>(() => new List<ICommonMarkExtension>(this.Extensions));
            clone.Reset();
            return clone;
        }

        internal void Reset()
        {
            this._inlineParserParameters = new Lazy<Parser.StandardInlineParserParameters>(GetInlineParserParameters);
            this._emphasisInlineParserParameters = new Lazy<Parser.EmphasisInlineParserParameters>(GetEmphasisInlineParserParameters);
            this._blockParserParameters = new Lazy<Parser.BlockParserParameters>(GetBlockParserParameters);
            this._blockFormatters = new Lazy<Formatters.IBlockFormatter[]>(GetBlockFormatters);
            this._inlineFormatters = new Lazy<Formatters.IInlineFormatter[]>(GetInlineFormatters);
        }

        #region [ Properties that cache parser parameters ]

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

        #region EmphasisInlineParserParameters

        private Lazy<Parser.EmphasisInlineParserParameters> _emphasisInlineParserParameters;

        /// <summary>
        /// Gets the parameters for parsing inline emphasis elements.
        /// </summary>
        public Parser.InlineParserParameters EmphasisInlineParserParameters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _emphasisInlineParserParameters.Value; }
        }

        private Parser.EmphasisInlineParserParameters GetEmphasisInlineParserParameters()
        {
            return new Parser.EmphasisInlineParserParameters();
        }

        #endregion EmphasisInlineParserParameters

        #region InlineParsers

        internal InlineParserDelegate[] GetInlineParsers()
        {
            return GetItems(Parser.InlineMethods.InitializeParsers(this),
                extension => extension.InlineParsers, key => key, GetInlineParser);
        }

        private static InlineParserDelegate GetInlineParser(InlineParserDelegate inner, InlineParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new Parser.DelegateInlineParser(inner, outer).ParseInline
                : inner;
        }

        #endregion InlineParsers

        #region InlineSingleCharTags

        internal Syntax.InlineTag[] GetInlineSingleCharTags()
        {
            return GetItems(Parser.InlineMethods.InitializeSingleCharTags(),
                ext => ext.SingleCharTags, key => key, GetInlineSingleCharTag);
        }

        private Syntax.InlineTag GetInlineSingleCharTag(Syntax.InlineTag inner, Syntax.InlineTag outer)
        {
            throw new InvalidOperationException(string.Format("Single character tag value is already set: {0}.", inner));
        }

        #endregion InlineSingleCharTags

        #region InlineDoubleCharTags

        internal Syntax.InlineTag[] GetInlineDoubleCharTags()
        {
            return GetItems(Parser.InlineMethods.InitializeDoubleCharTags(),
                ext => ext.DoubleCharTags, key => key, GetInlineDoubleCharTag);
        }

        private static Syntax.InlineTag GetInlineDoubleCharTag(Syntax.InlineTag inner, Syntax.InlineTag value)
        {
            throw new InvalidOperationException(string.Format("Double character tag value is already set: {0}.", inner));
        }

        #endregion InlineDoubleCharTags

        #endregion [ Properties that cache parser parameters ]

        #region [ Properties that cache formatter parameters ]

        #region FormatterParameters

        private Formatters.FormatterParameters _formatterParameters;

        /// <summary>
        /// Gets the formatter parameters.
        /// </summary>
        public Formatters.FormatterParameters FormatterParameters
        {
            get { return _formatterParameters; }
        }

        #endregion FormatterParameters

        #region BlockFormatters

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
            return GetItems(Formatters.BlockFormatter.InitializeFormatters(FormatterParameters),
                ext => ext.BlockFormatters, key => (int)key, GetBlockFormatter);
        }

        private static Formatters.IBlockFormatter GetBlockFormatter(Formatters.IBlockFormatter inner, Formatters.IBlockFormatter outer)
        {
            return !inner.Equals(outer)
                ? new Formatters.DelegateBlockFormatter(inner, outer)
                : inner;
        }

        #endregion BlockFormatters

        #region InlineFormatters

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
            return GetItems(Formatters.InlineFormatter.InitializeFormatters(FormatterParameters),
                ext => ext.InlineFormatters, key => (int)key, GetInlineFormatter);
        }

        private static Formatters.IInlineFormatter GetInlineFormatter(Formatters.IInlineFormatter inner, Formatters.IInlineFormatter outer)
        {
            return !inner.Equals(outer)
                ? new Formatters.DelegateInlineFormatter(inner, outer)
                : inner;
        }

        #endregion InlineFormatters

        #endregion [ Properties that cache formatter parameters ]

        #region Private helper methods

        private TValue[] GetItems<TKey, TValue>(TValue[] initialItems,
            Func<ICommonMarkExtension, IDictionary<TKey, TValue>> itemsFactory,
            Func<TKey, int> keyFactory, Func<TValue, TValue, TValue> valueFactory)
            where TKey: struct
        {
            foreach (var extension in Extensions)
            {
                var extensionItems = itemsFactory(extension);
                if (extensionItems != null)
                {
                    foreach (var kvp in extensionItems)
                    {
                        var value = kvp.Value;
                        if (value == null || value.Equals(default(TValue)))
                        {
                            var message = string.Format("{0} value cannot be {1}: {2}.", typeof(TValue).Name, 0, extension.ToString());
                            throw new InvalidOperationException(message);
                        }
                        var key = keyFactory(kvp.Key);
                        var inner = initialItems[key];
                        if (inner != null && !inner.Equals(default(TValue)))
                            value = valueFactory(inner, value);
                        initialItems[key] = value;
                    }
                }
            }
            return initialItems;
        }

        #endregion
    }
}
