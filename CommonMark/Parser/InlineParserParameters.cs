using CommonMark.Syntax;
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
        private readonly Lazy<InlineTag[]> _singleCharTags;
        private readonly Lazy<InlineTag[]> _doubleCharTags;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParserParameters"/> class.
        /// </summary>
        protected InlineParserParameters()
        {
            this._parsers = new Lazy<InlineParserDelegate[]>(GetParsers);
            this._specialCharacters = new Lazy<char[]>(GetSpecialCharacters);
            this._singleCharTags = new Lazy<InlineTag[]>(GetSingleCharTags);
            this._doubleCharTags = new Lazy<InlineTag[]>(GetDoubleCharTags);
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
        /// Gets the tags to use when single-character inline openers are matched.
        /// </summary>
        public InlineTag[] SingleCharTags
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _singleCharTags.Value; }
        }

        /// <summary>
        /// Gets the tags to use when double-character inline openers are matched.
        /// </summary>
        public InlineTag[] DoubleCharTags
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _doubleCharTags.Value; }
        }

        internal abstract InlineParserDelegate[] GetParsers();

        internal abstract InlineTag[] GetSingleCharTags();

        internal abstract InlineTag[] GetDoubleCharTags();

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

        internal override InlineTag[] GetSingleCharTags()
        {
            return Settings.GetItems(Parser.InlineMethods.InitializeSingleCharTags(),
                ext => ext.SingleCharTags, key => key, GetInlineSingleCharTag);
        }

        internal override InlineTag[] GetDoubleCharTags()
        {
            return Settings.GetItems(Parser.InlineMethods.InitializeDoubleCharTags(),
                ext => ext.DoubleCharTags, key => key, GetInlineDoubleCharTag);
        }

        private static InlineParserDelegate GetInlineParser(InlineParserDelegate inner, InlineParserDelegate outer)
        {
            return !inner.Equals(outer)
                ? new Parser.DelegateInlineParser(inner, outer).ParseInline
                : inner;
        }

        private static InlineTag GetInlineSingleCharTag(InlineTag inner, InlineTag outer)
        {
            throw new InvalidOperationException(string.Format("Single character tag value is already set: {0}.", inner));
        }

        private static InlineTag GetInlineDoubleCharTag(InlineTag inner, InlineTag value)
        {
            throw new InvalidOperationException(string.Format("Double character tag value is already set: {0}.", inner));
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

        internal override InlineTag[] GetSingleCharTags()
        {
            return Parser.InlineMethods.InitializeSingleCharTags();
        }

        internal override InlineTag[] GetDoubleCharTags()
        {
            return Parser.InlineMethods.InitializeDoubleCharTags();
        }
    }
}
