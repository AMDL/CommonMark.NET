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
            Settings = settings;
            CompleteNormalizeReference = DoCompleteNormalizeReference;

            Settings.Extensions.InitializeInlineParsing();

            this._delimiterHandlers = GetDelimiterHandlers();
            this._delimiterCharacters = GetDelimiterCharacters();
            this._parsers = GetParsers();
            this._handlers = GetHandlers();
            this._specialCharacters = GetSpecialCharacters();
            this._escapableCharacters = GetEscapableCharacters();
        }

        #endregion Constructor

        #region DelimiterHandlers

        private IInlineDelimiterHandler[] _delimiterHandlers;

        internal IInlineDelimiterHandler[] DelimiterHandlers
        {
            get { return _delimiterHandlers; }
        }

        internal abstract IInlineDelimiterHandler[] GetDelimiterHandlers();

        #endregion DelimiterHandlers

        #region DelimiterCharacters

        private readonly InlineDelimiterCharacterParameters[] _delimiterCharacters;

        /// <summary>
        /// Gets the parameters to use when inline openers are being matched.
        /// </summary>
        internal InlineDelimiterCharacterParameters[] DelimiterCharacters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _delimiterCharacters; }
        }

        private InlineDelimiterCharacterParameters[] GetDelimiterCharacters()
        {
            var handlers = DelimiterHandlers;
            var parameters = new InlineDelimiterCharacterParameters[handlers.Length];
            IInlineDelimiterHandler handler;
            for (var c = 0; c < handlers.Length; c++)
            {
                if ((handler = handlers[c]) != null)
                {
                    parameters[c] = GetDelimiterCharacter(handler);
                }
            }
            return parameters;
        }

        private static InlineDelimiterCharacterParameters GetDelimiterCharacter(IInlineDelimiterHandler handler)
        {
            return new InlineDelimiterCharacterParameters
            {
                SingleCharacter = GetDelimiter(handler, 1),
                DoubleCharacter = GetDelimiter(handler, 2),
            };
        }

        private static InlineDelimiterParameters GetDelimiter(IInlineDelimiterHandler handler, int delimiterCount)
        {
            var tag = handler.GetTag(delimiterCount);
            var parameters = new InlineDelimiterParameters(tag);
            if (tag != 0)
                parameters.Handler = GetHandler(handler);
            return parameters;
        }

        private static InlineDelimiterHandlerDelegate GetHandler(IInlineDelimiterHandler handler)
        {
            return (Subject subject, int startIndex, int length, CharacterType before, CharacterType after, ref bool canOpen, ref bool canClose) =>
                Handle(handler, subject, startIndex, length, before, after, ref canOpen, ref canClose);
        }

        private static bool Handle(IInlineDelimiterHandler handler, Subject subject, int startIndex, int length, CharacterType before, CharacterType after, ref bool canOpen, ref bool canClose)
        {
            bool couldOpen = canOpen;
            bool couldClose = canClose;
            canOpen &= handler.IsCanOpen(subject, startIndex, length, before, after, couldClose);
            canClose &= handler.IsCanClose(subject, startIndex, length, before, after, couldOpen);
            return true;
        }

        #endregion DelimiterCharacters

        #region Parsers

        private readonly IEnumerable<IInlineParser> _parsers;

        private IEnumerable<IInlineParser> Parsers
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _parsers; }
        }

        internal abstract IEnumerable<IInlineParser> GetParsers();

        #endregion Parsers

        #region Handlers

        private readonly InlineHandlerDelegate[] _handlers;

        /// <summary>
        /// Gets the delegates that parse inline elements.
        /// </summary>
        internal InlineHandlerDelegate[] Handlers
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _handlers; }
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

        private readonly char[] _specialCharacters;

        /// <summary>
        /// Gets the characters that have special meaning for inline element parsers.
        /// </summary>
        public char[] SpecialCharacters
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _specialCharacters; }
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

        #region EscapableCharacters

        private readonly bool[] _escapableCharacters;

        private bool[] GetEscapableCharacters()
        {
            return Settings.Extensions.GetItems(Utilities.InitializeEscapableCharacters(),
                ext => ext.EscapableCharacters, c => c, _ => true, (_1, _2) => true);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsEscapableCharacter(char c)
        {
            return c < _escapableCharacters.Length && _escapableCharacters[c];
        }

        #endregion EscapableCharacters

        #region ReferenceNormalizer

        /// <summary>
        /// Gets or sets the reference normalizer.
        /// </summary>
        internal StringNormalizerDelegate CompleteNormalizeReference
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get;
            set;
        }

        private string DoCompleteNormalizeReference(string s)
        {
            return s.ToUpperInvariant();
        }

        #endregion ReferenceNormalizer

        #region Settings

        /// <summary>
        /// Gets the common settings object.
        /// </summary>
        public CommonMarkSettings Settings
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

        internal override IInlineDelimiterHandler[] GetDelimiterHandlers()
        {
            return Settings.Extensions.GetItems(InlineParser.InitializeDelimiterHandlers,
                ext => ext.InlineDelimiterHandlers, key => key, DelegateInlineDelimiterHandler.Merge);
        }

        internal override IEnumerable<IInlineParser> GetParsers()
        {
            var parsers = new List<IInlineParser>(InlineParser.InitializeParsers(this));
            foreach (var ext in Settings.Extensions)
            {
                if (ext.InlineParsers != null)
                {
                    parsers.AddRange(ext.InlineParsers);
                }
            }
            return parsers.ToArray();
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
            return new IInlineParser[0];
        }

        internal override IInlineDelimiterHandler[] GetDelimiterHandlers()
        {
            return InlineParser.InitializeDelimiterHandlers(0);
        }
    }
}
