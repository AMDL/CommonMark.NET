using CommonMark.Formatters.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

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
        /// <param name="textTag">Text syntax tree tag. If unspecified, the first element of <paramref name="htmlTags"/> will be used.</param>
        /// <param name="htmlTags">HTML tags.</param>
        public BlockFormatter(FormatterParameters parameters, BlockTag tag, string textTag = null, params string[] htmlTags)
            : base(parameters, tag, textTag, htmlTags)
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
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public virtual bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            StartWriteOpening(writer, element);
            if (IsSelfClosing)
                writer.WriteLineConstant(" />");
            else
                writer.WriteLine('>');
            return !IsSelfClosing;
        }

        /// <summary>
        /// Writes the start of a block element opening.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        protected void StartWriteOpening(IHtmlTextWriter writer, Block element)
        {
            writer.EnsureLine();
            var value = string.Empty;
            for (int i = 0; i < HtmlTags.Length; i++)
            {
                if (value.Length > 0)
                    value += '>';
                value += "<" + HtmlTags[i];
            }
            writer.WriteConstant(value);
            WritePosition(writer, element);
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

        /// <summary>
        /// Determines whether paragraph rendering should be skipped for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> to skip paragraph rendering.</returns>
        public virtual bool IsTight(Block element)
        {
            return false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether handled elements are lists.
        /// </summary>
        public bool IsList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether handled elements are list items.
        /// </summary>
        public bool IsListItem
        {
            get;
            set;
        }

        internal static IEnumerable<IBlockFormatter> InitializeFormatters(FormatterParameters parameters)
        {
            yield return new DocumentFormatter(parameters);
            yield return new AtxHeadingFormatter(parameters);
            yield return new SetextHeadingFormatter(parameters);
            yield return new ParagraphFormatter(parameters);
            yield return new BlockQuoteFormatter(parameters);
            yield return new ThematicBreakFormatter(parameters);
            yield return new UnorderedListFormatter(parameters);
            yield return new OrderedListFormatter(parameters);
            yield return new ListItemFormatter(parameters);
            yield return new FencedCodeFormatter(parameters);
            yield return new IndentedCodeFormatter(parameters);
            yield return new HtmlBlockFormatter(parameters);
            yield return new ReferenceDefinitionFormatter(parameters);
        }
    }
}
