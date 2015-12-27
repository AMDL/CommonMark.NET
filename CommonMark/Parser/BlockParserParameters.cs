using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    /// <summary>
    /// Stage 1 block initializer delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockInitializerDelegate(ref BlockParserInfo info);

    /// <summary>
    /// Stage 1 block opener delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockOpenerDelegate(ref BlockParserInfo info);

    /// <summary>
    /// Stage 1 block closer delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockCloserDelegate(BlockParserInfo info);

    /// <summary>
    /// Stage 1 block finalizer delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockFinalizerDelegate(Block container);

    /// <summary>
    /// Stage 1 block blank line check delegate.
    /// </summary>
    /// <param name="info">Parser state.</param>
    /// <returns><c>true</c> to discard the last blank line.</returns>
    internal delegate bool IsDiscardLastBlankDelegate(BlockParserInfo info);

    /// <summary>
    /// Stage 2 block processor delegate.
    /// </summary>
    /// <param name="container">Container element.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="inlineStack">Inline stack.</param>
    /// <returns><c>true</c> if successful.</returns>
    internal delegate bool BlockProcessorDelegate(Block container, Subject subject, ref Stack<Inline> inlineStack);

    /// <summary>
    /// Block element parser parameters.
    /// </summary>
    public class BlockParserParameters
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockParserParameters"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public BlockParserParameters(CommonMarkSettings settings)
        {
            this.Settings = settings;
            this._parsers = GetParsers();
            this._handlers = GetHandlers();
            this._initializers = GetInitializers();
            this._openers = GetOpeners();
            this._closers = GetClosers();
            this._finalizers = GetFinalizers();
            this._processors = GetProcessors();
            this._isDiscardLastBlanks = GetIsDiscardLastBlanks();
            this._isList = GetIsList();
            this._isCodeBlock = GetIsCodeBlock();
            this._isAcceptsLines = GetIsAcceptsLines();
            this._isAlwaysDiscardBlanks = GetIsAlwaysDiscardBlanks();
            this._canContain = GetCanContain();
            this._openParagraph = GetOpenParagraph();
            this._closeParagraph = GetCloseParagraph();
            this._openIndentedCode = GetOpenIndentedCode();
        }

        #endregion Constructor

        #region Parsers

        private readonly IBlockParser[] _parsers;

        /// <summary>
        /// Gets the array mapping from block tag to block element parser.
        /// </summary>
        /// <value>Mapping from block element tag to parser.</value>
        public IBlockParser[] Parsers
        {
            get { return _parsers; }
        }

        private IBlockParser[] GetParsers()
        {
            return Settings.Extensions.GetItems(BlockParser.InitializeParsers(Settings), (int)BlockTag.Count,
                ext => ext.BlockParsers, p => (int)p.Tag, (i, o) => DelegateBlockParser.Merge(o.Tag, i, o),
                i => null);
        }

        #endregion Parsers

        #region Handlers

        private readonly IBlockDelimiterHandler[] _handlers;

        internal IBlockDelimiterHandler[] Handlers
        {
            get { return _handlers; }
        } 

        private IBlockDelimiterHandler[] GetHandlers()
        {
            IEnumerable<IBlockDelimiterHandler> itemHandlers;
            var allHandlers = new List<IBlockDelimiterHandler>();
            foreach (var parser in Parsers)
            {
                if ((itemHandlers = parser?.Handlers) != null)
                {
                    allHandlers.AddRange(itemHandlers);
                }
            }

            foreach (var ext in Settings.Extensions)
	        {
                if ((itemHandlers = ext.BlockDelimiterHandlers) != null)
                {
                    allHandlers.AddRange(itemHandlers);
                }
	        }

            var i = new Dictionary<char, IBlockDelimiterHandler>();
            var max = (char)0;
            IBlockDelimiterHandler inner;
            foreach (var handler in allHandlers)
            {
                foreach (var c in handler.Characters)
                {
                    i.TryGetValue(c, out inner);
                    i[c] = DelegateBlockDelimiterHandler.Merge(inner, handler, c);
                    if (c > max)
                        max = c;
                }
            }

            var handlers = new IBlockDelimiterHandler[max + 1];
            foreach (var kvp in i)
            {
                handlers[kvp.Key] = kvp.Value;
            }
            return handlers;
        }

        #endregion Handlers

        #region Initializers

        private readonly BlockInitializerDelegate[] _initializers;

        /// <summary>
        /// Gets the initializer delegates.
        /// </summary>
        /// <value>Mapping from block element to initializer delegate.</value>
        internal BlockInitializerDelegate[] Initializers
        {
            get { return _initializers; }
        }

        private BlockInitializerDelegate[] GetInitializers()
        {
            return GetItems<BlockInitializerDelegate>(p => p.Initialize);
        }

        #endregion Initializers

        #region Openers

        private readonly BlockOpenerDelegate[] _openers;

        /// <summary>
        /// Gets the opener delegates.
        /// </summary>
        /// <value>Mapping from character tag to opener delegate.</value>
        internal BlockOpenerDelegate[] Openers
        {
            get { return _openers; }
        }

        private BlockOpenerDelegate[] GetOpeners()
        {
            var openers = new BlockOpenerDelegate[Handlers.Length];
            for (int i = 0; i < Handlers.Length; i++)
            {
                var handler = Handlers[i];
                if (handler != null)
                    openers[i] = handler.Handle;
            }
            return openers;
        }

        #endregion Openers

        #region Closers

        private readonly BlockCloserDelegate[] _closers;

        /// <summary>
        /// Gets the closer delegates.
        /// </summary>
        /// <value>Mapping from block element tag to closer delegate.</value>
        internal BlockCloserDelegate[] Closers
        {
            get { return _closers; }
        }

        private BlockCloserDelegate[] GetClosers()
        {
            return GetItems<BlockCloserDelegate>(p => p.Close);
        }

        #endregion Closers

        #region Finalizers

        private readonly BlockFinalizerDelegate[] _finalizers;

        /// <summary>
        /// Gets the finalizer delegates.
        /// </summary>
        /// <value>Mapping from block element tag to finalizer delegate.</value>
        internal BlockFinalizerDelegate[] Finalizers
        {
            get { return _finalizers; }
        }

        private BlockFinalizerDelegate[] GetFinalizers()
        {
            return GetItems<BlockFinalizerDelegate>(p => p.Finalize);
        }

        #endregion Finalizers

        #region Processors

        private readonly BlockProcessorDelegate[] _processors;

        /// <summary>
        /// Gets the processor delegates.
        /// </summary>
        /// <value>Mapping from block element tag to processor delegate.</value>
        internal BlockProcessorDelegate[] Processors
        {
            get { return _processors; }
        }

        private BlockProcessorDelegate[] GetProcessors()
        {
            return GetItems<BlockProcessorDelegate>(p => p.Process);
        }

        #endregion Processors

        #region IsList

        private long _isList; // assuming we won't get past 63 tags

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsList(BlockTag tag)
        {
            return 0 != (_isList & 1 << (int)tag);
        }

        private long GetIsList()
        {
            return GetValue(GetIsList);
        }

        private bool GetIsList(BlockTag tag)
        {
            IBlockParser parser;
            return (parser = Parsers[(int)tag]) != null && parser.IsList;
        }

        #endregion IsList

        #region IsCodeBlock

        private long _isCodeBlock;

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsCodeBlock(BlockTag tag)
        {
            return 0 != (_isCodeBlock & 1 << (int)tag);
        }

        private long GetIsCodeBlock()
        {
            return GetValue(GetIsCodeBlock);
        }

        private bool GetIsCodeBlock(BlockTag tag)
        {
            IBlockParser parser;
            return (parser = Parsers[(int)tag]) != null && parser.IsCodeBlock;
        }

        #endregion IsCodeBlock

        #region IsAcceptsLines

        private long _isAcceptsLines;

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsAcceptsLines(BlockTag tag)
        {
            return 0 != (_isAcceptsLines & 1 << (int)tag);
        }

        private long GetIsAcceptsLines()
        {
            return GetValue(GetIsAcceptsLines);
        }

        private bool GetIsAcceptsLines(BlockTag tag)
        {
            IBlockParser parser;
            return (parser = Parsers[(int)tag]) != null && parser.IsAcceptsLines;
        }

        #endregion IsAcceptsLines

        #region IsDiscardLastBlank

        private IsDiscardLastBlankDelegate[] _isDiscardLastBlanks;

        internal IsDiscardLastBlankDelegate[] IsDiscardLastBlanks
        {
            get { return _isDiscardLastBlanks; }
        }

        private IsDiscardLastBlankDelegate[] GetIsDiscardLastBlanks()
        {
            return GetItems<IsDiscardLastBlankDelegate>(p => p.IsDiscardLastBlank);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsDiscardLastBlank(BlockParserInfo info)
        {
            var tag = info.Container.Tag;
            IsDiscardLastBlankDelegate isDiscard;
            return IsAlwaysDiscardBlanks(tag) || ((isDiscard = IsDiscardLastBlanks[(int)tag]) != null && isDiscard(info));
        }

        private long _isAlwaysDiscardBlanks;

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsAlwaysDiscardBlanks(BlockTag tag)
        {
            return 0 != (_isAlwaysDiscardBlanks & 1 << (int)tag);
        }

        private long GetIsAlwaysDiscardBlanks()
        {
            return GetValue(GetIsAlwaysDiscardBlanks);
        }

        private bool GetIsAlwaysDiscardBlanks(BlockTag tag)
        {
            IBlockParser parser;
            return (parser = Parsers[(int)tag]) != null && parser.IsAlwaysDiscardBlanks;
        }

        #endregion IsDiscardLastBlank

        #region CanContain

        private long[] _canContain; // assuming we won't get past 63 tags

#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal bool CanContain(BlockTag parentTag, BlockTag childTag)
        {
            return 0 != (_canContain[(int)parentTag] & (1 << (int)childTag));
        }

        private long[] GetCanContain()
        {
            var canContain = new long[(int)BlockTag.Count];
            IBlockParser parser;
            for (var i = 0; i < (int)BlockTag.Count; i++)
            {
                if ((parser = Parsers[i]) != null)
                {
                    canContain[i] = GetValue(parser.CanContain);
                }
            }
            return canContain;
        }

        #endregion CanContain

        #region OpenParagraph

        private BlockOpenerDelegate _openParagraph;

        internal BlockOpenerDelegate OpenParagraph
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _openParagraph; }
        }

        private BlockOpenerDelegate GetOpenParagraph()
        {
            var handlers = new List<IBlockDelimiterHandler>(Parsers[(int)BlockTag.Paragraph].Handlers);
            return handlers[0].Handle;
        }

        #endregion OpenParagraph

        #region CloseParagraph

        private BlockCloserDelegate _closeParagraph;

        internal BlockCloserDelegate CloseParagraph
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _closeParagraph; }
        }

        private BlockCloserDelegate GetCloseParagraph()
        {
            return Parsers[(int)BlockTag.Paragraph].Close;
        }

        #endregion OpenParagraph

        #region OpenIndentedCode

        private BlockOpenerDelegate _openIndentedCode;

        internal BlockOpenerDelegate OpenIndentedCode
        {
#if OptimizeFor45
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            get { return _openIndentedCode; }
        }

        private BlockOpenerDelegate GetOpenIndentedCode()
        {
            var handlers = new List<IBlockDelimiterHandler>(Parsers[(int)BlockTag.IndentedCode].Handlers);
            return handlers[0].Handle;
        }

        #endregion OpenIndentedCode

        /// <summary>
        /// Returns the parser delegates of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of delegate.</typeparam>
        /// <param name="itemFactory">Item factory.</param>
        /// <returns>Mapping from block element tag to <typeparamref name="T"/>.</returns>
        protected T[] GetItems<T>(Func<IBlockParser, T> itemFactory)
        {
            var parsers = Parsers;
            var items = new T[(int)BlockTag.Count];
            for (int i = 0; i < (int)BlockTag.Count; i++)
            {
                if (parsers[i] != null)
                {
                    items[i] = itemFactory(parsers[i]);
                }
            }
            return items;
        }

        /// <summary>
        /// Compresseses values obtained from the specified value factory into a bit mask.
        /// </summary>
        /// <param name="valueFactory">Value factory.</param>
        /// <returns>Int64.</returns>
        protected long GetValue(Func<BlockTag, bool> valueFactory)
        {
            long c = 0, m = 1;
            for (var i = 0; i < (int)BlockTag.Count; i++)
            {
                if (valueFactory((BlockTag)i))
                {
                    c |= m;
                }
                m <<= 1;
            }
            return c;
        }

        private CommonMarkSettings Settings { get; }
    }
}
