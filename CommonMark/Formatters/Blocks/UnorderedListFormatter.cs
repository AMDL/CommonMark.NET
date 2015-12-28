using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.UnorderedList"/> element formatter.
    /// </summary>
    public class UnorderedListFormatter : ListFormatter<UnorderedListData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedListFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="textTag">Text syntax tree tag.</param>
        public UnorderedListFormatter(FormatterParameters parameters, BlockTag tag = BlockTag.UnorderedList, string textTag = "unordered_list")
            : base(parameters, tag, "ul", textTag)
        {
        }

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element)
        {
            StartWriteOpening(writer, element);
            return CompleteWriteOpening(writer, element, element.UnorderedList);
        }

        /// <summary>
        /// Determines whether paragraph rendering should be skipped for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <returns><c>true</c> to skip paragraph rendering.</returns>
        public override bool IsTight(Block element)
        {
            return element.UnorderedList.IsTight;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block element)
        {
            return new Dictionary<string, object>
            {
                { "tight", element.UnorderedList.IsTight },
                { "bullet_char", element.UnorderedList.BulletCharacter },
            };
        }
    }

    internal sealed class LegacyUnorderedListFormatter : UnorderedListFormatter
    {
        public LegacyUnorderedListFormatter(FormatterParameters parameters)
#pragma warning disable 0618
            : base(parameters, BlockTag.List, "list")
#pragma warning restore 0618
        {
        }

        public override bool CanHandle(Block element)
        {
#pragma warning disable 0618
            return element.Tag == BlockTag.List && element.ListData.ListType == ListType.Bullet;
#pragma warning restore 0618
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="formatter">Syntax formatter.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block element)
        {
            return new Dictionary<string, object>
            {
                { "type", "bullet" },
#pragma warning disable 0618
                { "tight", element.ListData.IsTight },
                { "bullet_char", element.ListData.BulletChar },
#pragma warning restore 0618
            };
        }
    }
}
