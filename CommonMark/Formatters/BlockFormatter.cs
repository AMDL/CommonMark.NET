﻿using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// Block element formatter.
    /// </summary>
    public class BlockFormatter : ElementFormatter<Block, BlockTag>, IBlockFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        public BlockFormatter(FormatterParameters parameters, BlockTag tag, string htmlTag = null)
            : base(parameters, tag, htmlTag)
        {
        }

        /// <summary>
        /// Checks whether the formatter can handle a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the formatter can handle <paramref name="element"/>.</returns>
        public override bool CanHandle(Block element)
        {
            return element.Tag == Tag;
        }

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="block">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        public virtual bool? IsStackTight(Block block, bool tight)
        {
            return null;
        }

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public virtual bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            writer.EnsureLine();
            return DoWriteOpening(writer, element);
        }

        /// <summary>
        /// Returns the closing of a block element.
        /// </summary>
        /// <param name="formatter">HTML formatter.</param>
        /// <param name="element">Block element.</param>
        /// <returns>The closing.</returns>
        public virtual string GetClosing(IHtmlFormatter formatter, Block element)
        {
            return base.DoGetClosing(element);
        }

        /// <summary>
        /// Writes the position of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        protected override void DoWritePosition(IHtmlTextWriter writer, Block element)
        {
            writer.WritePosition(element);
        }

        internal static IBlockFormatter[] InitializeFormatters(FormatterParameters parameters)
        {
            var f = new IBlockFormatter[(int)BlockTag.Count];
            f[(int)BlockTag.FencedCode] = new FencedCodeFormatter(parameters);
            f[(int)BlockTag.IndentedCode] = new IndentedCodeFormatter(parameters);
            return f;
        }
    }
}
