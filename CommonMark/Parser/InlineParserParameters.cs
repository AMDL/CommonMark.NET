using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline parser parameters.
    /// </summary>
    public abstract class InlineParserParameters
    {
        private readonly Lazy<InlineParserDelegate[]> _parsers;
        private readonly Lazy<char[]> _specialCharacters;
        private readonly Lazy<InlineDelimiterCharacterParameters[]> _delimiters;
        private readonly Lazy<StringNormalizerDelegate> _referenceNormalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParserParameters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        protected InlineParserParameters(CommonMarkSettings settings)
        {
            this._parsers = settings.GetLazy(GetParsers);
            this._specialCharacters = settings.GetLazy(GetSpecialCharacters);
            this._delimiters = settings.GetLazy(GetDelimiterCharacters);
            this._referenceNormalizer = settings.GetLazy(GetReferenceNormalizer);
        }

        /// <summary>
        /// Gets the delegates that parse inline elements.
        /// </summary>
        public InlineParserDelegate[] Parsers
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _parsers.Value; }
        }

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

        /// <summary>
        /// Gets the parameters to use when inline openers are being matched.
        /// </summary>
        public InlineDelimiterCharacterParameters[] DelimiterCharacters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _delimiters.Value; }
        }

        /// <summary>
        /// Gets the reference normalizer.
        /// </summary>
        public StringNormalizerDelegate ReferenceNormalizer
        {
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

        internal abstract InlineParserDelegate[] GetParsers();

        internal abstract InlineDelimiterCharacterParameters[] GetDelimiterCharacters();

        private char[] GetSpecialCharacters()
        {
            var p = this.Parsers;
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
    }

    /// <summary>
    /// Parameters for parsing inline elements according to the specified settings.
    /// </summary>
    internal class StandardInlineParserParameters : InlineParserParameters
    {
        public StandardInlineParserParameters(CommonMarkSettings settings)
            : base(settings)
        {
            this.Settings = settings;
        }

        internal override InlineParserDelegate[] GetParsers()
        {
            return Settings.Extensions.GetItems(Parser.InlineMethods.InitializeParsers(this),
                ext => ext.InlineParsers, key => key, DelegateInlineParser.Merge);
        }

        internal override InlineDelimiterCharacterParameters[] GetDelimiterCharacters()
        {
            return Settings.Extensions.GetItems(Parser.InlineMethods.InitializeDelimiterCharacters,
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

        private CommonMarkSettings Settings
        {
            get;
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

        internal override InlineParserDelegate[] GetParsers()
        {
            return Parser.InlineMethods.InitializeEmphasisParsers(this);
        }

        internal override InlineDelimiterCharacterParameters[] GetDelimiterCharacters()
        {
            return Parser.InlineMethods.EmphasisDelimiterCharacters;
        }
    }
}
