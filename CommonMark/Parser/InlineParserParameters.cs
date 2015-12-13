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

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParserParameters"/> class.
        /// </summary>
        protected InlineParserParameters()
        {
            this._parsers = new Lazy<InlineParserDelegate[]>(GetParsers);
            this._specialCharacters = new Lazy<char[]>(GetSpecialCharacters);
            this._delimiters = new Lazy<InlineDelimiterCharacterParameters[]>(GetDelimiterCharacters);
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
        private readonly CommonMarkSettings _settings;

        public StandardInlineParserParameters(CommonMarkSettings settings)
        {
            this._settings = settings;
        }

        internal override InlineParserDelegate[] GetParsers()
        {
            return Settings.GetItems(Parser.InlineMethods.InitializeParsers(this),
                extension => extension.InlineParsers, key => key, GetParser);
        }

        internal override InlineDelimiterCharacterParameters[] GetDelimiterCharacters()
        {
            return Settings.GetItems(Parser.InlineMethods.InitializeDelimiterCharacters(),
                ext => ext.InlineDelimiterCharacters, key => key, GetDelimiterCharacter);
        }

        private static InlineParserDelegate GetParser(InlineParserDelegate inner, InlineParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new Parser.DelegateInlineParser(inner, outer).ParseInline
                : inner;
        }

        private static InlineDelimiterCharacterParameters GetDelimiterCharacter(InlineDelimiterCharacterParameters inner, InlineDelimiterCharacterParameters outer)
        {
            return new InlineDelimiterCharacterParameters
            {
                SingleCharacter = GetDelimiter(inner.SingleCharacter, outer.SingleCharacter, "Single character"),
                DoubleCharacter = GetDelimiter(inner.DoubleCharacter, outer.DoubleCharacter, "Double character"),
            };
        }

        private static InlineDelimiterParameters GetDelimiter(InlineDelimiterParameters inner, InlineDelimiterParameters outer, string key)
        {
            if (!inner.IsEmpty && !outer.IsEmpty)
                throw new InvalidOperationException(string.Format("{0} parameters value is already set: {1}.", key, inner));
            return !inner.IsEmpty ? inner : outer;
        }

        private CommonMarkSettings Settings
        {
            get { return _settings; }
        }
    }

    /// <summary>
    /// Parameters for parsing inline emphasis elements.
    /// </summary>
    internal class EmphasisInlineParserParameters : InlineParserParameters
    {
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
