﻿using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    /// <summary>
    /// Block parser.
    /// </summary>
    public interface IBlockParser
    {
        /// <summary>
        /// Gets the element tag.
        /// </summary>
        /// <value>The element tag handled by this parser.</value>
        BlockTag Tag { get; }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        IEnumerable<IBlockDelimiterHandler> Handlers { get; }

        /// <summary>
        /// Gets the value indicating whether a handled element is a list.
        /// </summary>
        bool IsList { get; }

        /// <summary>
        /// Gets the value indicating whether a handled element is a code block.
        /// </summary>
        /// <value><c>true</c> if a handled element is a code block.</value>
        bool IsCodeBlock { get; }

        /// <summary>
        /// Gets the value indicating whether a handled element accepts new lines.
        /// </summary>
        /// <value><c>true</c> if new lines can be added to a handled element.</value>
        bool IsAcceptsLines { get; }

        /// <summary>
        /// Gets or sets the value indicating whether the last blank line of a handled element should be discarded.
        /// </summary>
        /// <value><c>true</c> if a blank line at the end of a handled element should always be discarded.</value>
        bool IsAlwaysDiscardBlanks { get; }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if blank lines at the end of the handled element should be discarded.</returns>
        bool IsDiscardLastBlank(BlockParserInfo info);

        /// <summary>
        /// Determines whether a handled element can contain child elements of the specified kind.
        /// </summary>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if handled elements can contain elements having <paramref name="childTag"/>.</returns>
        bool CanContain(BlockTag childTag);

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        bool Initialize(ref BlockParserInfo info);

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        bool Close(BlockParserInfo info);

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        bool Finalize(Block container);

        /// <summary>
        /// Processes the inline contents of a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <returns><c>true</c> if successful.</returns>
        bool Process(Block container, Subject subject, ref Stack<Inline> inlineStack);
    }
}