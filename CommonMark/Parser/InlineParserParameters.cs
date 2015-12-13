using CommonMark.Syntax;
using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Inline stack delimiter parameters.
    /// </summary>
    public struct InlineDelimiterParameters
    {
        /// <summary>
        /// Empty parameters instance.
        /// </summary>
        public static readonly InlineDelimiterParameters Empty = new InlineDelimiterParameters();

        /// <summary>
        /// Determines whether the parameters instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return Tag == 0; }
        }

        /// <summary>
        /// The tag to use for the inline element when the opener is matched.
        /// </summary>
        public InlineTag Tag { get; set; }
    }

    /// <summary>
    /// Inline parser parameters.
    /// </summary>
    public abstract class InlineParserParameters
    {
        private readonly Lazy<InlineParserDelegate[]> _parsers;
        private readonly Lazy<char[]> _specialCharacters;
        private readonly Lazy<InlineDelimiterParameters[]> _singleChars;
        private readonly Lazy<InlineDelimiterParameters[]> _doubleChars;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParserParameters"/> class.
        /// </summary>
        protected InlineParserParameters()
        {
            this._parsers = new Lazy<InlineParserDelegate[]>(GetParsers);
            this._specialCharacters = new Lazy<char[]>(GetSpecialCharacters);
            this._singleChars = new Lazy<InlineDelimiterParameters[]>(GetSingleChars);
            this._doubleChars = new Lazy<InlineDelimiterParameters[]>(GetDoubleChars);
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
        /// Gets the parameters to use when single-character inline openers are being matched.
        /// </summary>
        public InlineDelimiterParameters[] SingleChars
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _singleChars.Value; }
        }

        /// <summary>
        /// Gets the parameters to use when double-character inline openers are being matched.
        /// </summary>
        public InlineDelimiterParameters[] DoubleChars
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _doubleChars.Value; }
        }

        internal abstract InlineParserDelegate[] GetParsers();

        internal abstract InlineDelimiterParameters[] GetSingleChars();

        internal abstract InlineDelimiterParameters[] GetDoubleChars();

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
            return Settings.GetItems(Parser.InlineMethods.InitializeParsers(Settings),
                extension => extension.InlineParsers, key => key, GetInlineParser);
        }

        internal override InlineDelimiterParameters[] GetSingleChars()
        {
            return Settings.GetItems(Parser.InlineMethods.InitializeSingleChars(),
                ext => ext.InlineSingleChars, key => key, GetInlineSingleChar);
        }

        internal override InlineDelimiterParameters[] GetDoubleChars()
        {
            return Settings.GetItems(Parser.InlineMethods.InitializeDoubleChars(),
                ext => ext.InlineDoubleChars, key => key, GetInlineDoubleChar);
        }

        private static InlineParserDelegate GetInlineParser(InlineParserDelegate inner, InlineParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new Parser.DelegateInlineParser(inner, outer).ParseInline
                : inner;
        }

        private static InlineDelimiterParameters GetInlineSingleChar(InlineDelimiterParameters inner, InlineDelimiterParameters outer)
        {
            throw new InvalidOperationException(string.Format("Single character parameters value is already set: {0}.", inner));
        }

        private static InlineDelimiterParameters GetInlineDoubleChar(InlineDelimiterParameters inner, InlineDelimiterParameters value)
        {
            throw new InvalidOperationException(string.Format("Double character parameters value is already set: {0}.", inner));
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
            return Parser.InlineMethods.EmphasisParsers;
        }

        internal override InlineDelimiterParameters[] GetSingleChars()
        {
            return Parser.InlineMethods.EmphasisSingleChars;
        }

        internal override InlineDelimiterParameters[] GetDoubleChars()
        {
            return Parser.InlineMethods.EmphasisDoubleChars;
        }
    }
}
