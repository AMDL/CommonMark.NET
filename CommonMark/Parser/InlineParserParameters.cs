using System;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline handler delegate.
    /// </summary>
    /// <param name="parent">Parent container.</param>
    /// <param name="subject">Subject.</param>
    /// <returns>Inline element or <c>null</c>.</returns>
    internal delegate Syntax.Inline InlineHandlerDelegate(Syntax.Block parent, Subject subject);

    /// <summary>
    /// String normalizer delegate.
    /// </summary>
    /// <param name="s">String.</param>
    /// <returns>Normalized string.</returns>
    public delegate string StringNormalizerDelegate(string s);

    /// <summary>
    /// Inline parser parameters.
    /// </summary>
    public abstract class InlineParserParameters
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParserParameters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected InlineParserParameters(CommonMarkSettings settings)
        {
            this.Settings = settings;
            this._parsers = Settings.GetLazy(GetParsers);
            this._handlers = Settings.GetLazy(GetHandlers);
            this._specialCharacters = Settings.GetLazy(GetSpecialCharacters);
            this._delimiterCharacters = Settings.GetLazy(GetDelimiterCharacters);
            this._referenceNormalizer = Settings.GetLazy(GetReferenceNormalizer);
        }

        #endregion Constructor

        #region Parsers

        private readonly Lazy<IEnumerable<IInlineParser>> _parsers;

        private IEnumerable<IInlineParser> Parsers
        {
            get { return _parsers.Value; }
        }

        internal abstract IEnumerable<IInlineParser> GetParsers();

        #endregion Parsers

        #region Handlers

        private readonly Lazy<InlineHandlerDelegate[]> _handlers;

        /// <summary>
        /// Gets the delegates that parse inline elements.
        /// </summary>
        internal InlineHandlerDelegate[] Handlers
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _handlers.Value; }
        }

        private InlineHandlerDelegate[] GetHandlers()
        {
            var parsers = Parsers;
            var i = new Dictionary<char, InlineHandlerDelegate>();
            var max = (char)0;
            InlineHandlerDelegate handler;
            foreach (var parser in parsers)
            {
                var c = parser.Character;
                i.TryGetValue(c, out handler);
                i[c] = DelegateInlineHandler.Merge(handler, parser.Handle);
                if (c > max)
                    max = c;
            }

            var handlers = new InlineHandlerDelegate[max + 1];
            foreach (var kvp in i)
            {
                handlers[kvp.Key] = kvp.Value;
            }

            return handlers;
        }

        #endregion Handlers

        #region SpecialCharacters

        private readonly Lazy<char[]> _specialCharacters;

        /// <summary>
        /// Gets the characters that have special meaning for inline element parsers.
        /// </summary>
        public char[] SpecialCharacters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _specialCharacters.Value; }
        }

        private char[] GetSpecialCharacters()
        {
            var p = this.Handlers;
            var l = p.Length;

            var m = 0;
            for (var i = 0; i < l; i++)
                if (p[i] != null)
                    m++;

            var s = new char[m];
            var j = 0;
            for (var i = 0; i < l; i++)
                if (p[i] != null)
                    s[j++] = (char)i;

            return s;
        }

        #endregion SpecialCharacters

        #region DelimiterCharacters

        private readonly Lazy<InlineDelimiterCharacterParameters[]> _delimiterCharacters;

        /// <summary>
        /// Gets the parameters to use when inline openers are being matched.
        /// </summary>
        public InlineDelimiterCharacterParameters[] DelimiterCharacters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _delimiterCharacters.Value; }
        }

        internal abstract InlineDelimiterCharacterParameters[] GetDelimiterCharacters();

        #endregion DelimiterCharacters

        #region ReferenceNormalizer

        private readonly Lazy<StringNormalizerDelegate> _referenceNormalizer;

        /// <summary>
        /// Gets the reference normalizer.
        /// </summary>
        public StringNormalizerDelegate ReferenceNormalizer
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _referenceNormalizer.Value; }
        }

        /// <summary>
        /// Creates the reference normalizer.
        /// </summary>
        /// <returns>Reference normalizer delegate.</returns>
        protected virtual StringNormalizerDelegate GetReferenceNormalizer()
        {
            return s => s.ToUpperInvariant();
        }

        #endregion ReferenceNormalizer

        #region Settings

        /// <summary>
        /// Gets the common settings object.
        /// </summary>
        protected CommonMarkSettings Settings
        {
            get;
        }

        #endregion Settings
    }

    /// <summary>
    /// Parameters for parsing inline elements according to the specified settings.
    /// </summary>
    internal class StandardInlineParserParameters : InlineParserParameters
    {
        public StandardInlineParserParameters(CommonMarkSettings settings)
            : base(settings)
        {
        }

        internal override IEnumerable<IInlineParser> GetParsers()
        {
            var parsers = new List<IInlineParser>(InlineParser.InitializeParsers(Settings));
            foreach (var ext in Settings.Extensions)
            {
                if (ext.InlineParsers != null)
                {
                    parsers.AddRange(ext.InlineParsers);
                }
            }
            return parsers.ToArray();
        }

        internal override InlineDelimiterCharacterParameters[] GetDelimiterCharacters()
        {
            return Settings.Extensions.GetItems(InlineParser.InitializeDelimiterCharacters(),
                ext => ext.InlineDelimiterCharacters, key => key, InlineDelimiterCharacterParameters.Merge);
        }

        /// <summary>
        /// Creates the reference normalizer.
        /// </summary>
        /// <returns>Reference normalizer delegate.</returns>
        protected override StringNormalizerDelegate GetReferenceNormalizer()
        {
            StringNormalizerDelegate normalizer;
            foreach (var ext in Settings.Extensions)
                if ((normalizer = ext.ReferenceNormalizer) != null)
                    return normalizer;
            return base.GetReferenceNormalizer();
        }
    }

    /// <summary>
    /// Parameters for parsing inline emphasis elements.
    /// </summary>
    public class EmphasisInlineParserParameters : InlineParserParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisInlineParserParameters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public EmphasisInlineParserParameters(CommonMarkSettings settings)
            : base(settings)
        {
        }

        internal override IEnumerable<IInlineParser> GetParsers()
        {
            return InlineParser.InitializeEmphasisParsers(this, Settings);
        }

        internal override InlineDelimiterCharacterParameters[] GetDelimiterCharacters()
        {
            return InlineParser.EmphasisDelimiterCharacters;
        }
    }
}
