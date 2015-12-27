﻿using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Setext heading parameters.
    /// </summary>
    public sealed class SetextHeadingParameters
    {
        /// <summary>
        /// Gets or sets the delimiter parameters.
        /// </summary>
        public SetextHeadingDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.SetextHeading"/> element parser.
    /// </summary>
    public sealed class SetextHeadingParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly SetextHeadingParameters DefaultParameters = new SetextHeadingParameters
        {
            Delimiters = new[]
            {
                new SetextHeadingDelimiterParameters('=', 1),
                new SetextHeadingDelimiterParameters('-', 2),
            },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SetextHeadingParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Setext heading parameters.</param>
        public SetextHeadingParser(CommonMarkSettings settings, SetextHeadingParameters parameters = null)
            : base(settings, BlockTag.SetextHeading)
        {
            // we don't count setext headings for purposes of tight/loose lists or breaking out of lists.
            IsAlwaysDiscardBlanks = true;

            Parameters = parameters ?? DefaultParameters;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                foreach (var delimiter in Parameters.Delimiters)
                {
                    yield return new SetextHeadingHandler(Settings, Tag, delimiter);
                }
            }
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            // a heading can never contain more than one line
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
            }
            return false;
        }

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Close(BlockParserInfo info)
        {
            return true;
        }

        /// <summary>
        /// Processes the inline contents of a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Process(Block container, Subject subject, ref Stack<Inline> inlineStack)
        {
            return ProcessInlines(container, subject, ref inlineStack, Settings.InlineParserParameters);
        }

        private SetextHeadingParameters Parameters
        {
            get;
        }
    }
}
