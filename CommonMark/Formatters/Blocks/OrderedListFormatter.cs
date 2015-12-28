using CommonMark.Syntax;
using System.Collections.Generic;
using System.Globalization;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.OrderedList"/> element formatter.
    /// </summary>
    public class OrderedListFormatter : ListFormatter<OrderedListData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="textTag">Text syntax tree tag.</param>
        public OrderedListFormatter(FormatterParameters parameters, BlockTag tag = BlockTag.OrderedList, string textTag = "ordered_list")
            : base(parameters, tag, "ol", textTag)
        {
        }

        /// <summary>
        /// Writes the opening of a block element.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">Block element.</param>
        /// <param name="tight"><c>true</c> to stack paragraphs tightly.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        public override bool WriteOpening(IHtmlTextWriter writer, Block element, bool tight)
        {
            var list = element.OrderedList;
            StartWriteOpening(writer, element);
            if (list.Start != 1)
            {
                writer.WriteConstant(" start=\"");
                writer.WriteConstant(list.Start.ToString(CultureInfo.InvariantCulture));
                writer.Write('\"');
            }
            if (list.MarkerType != 0 && Parameters.IsOutputListTypes)
            {
                writer.WriteConstant(" type=\"");
                writer.Write((char)list.MarkerType);
                writer.Write('\"');
            }
            return CompleteWriteOpening(writer, element, list);
        }

        /// <summary>
        /// Determines whether paragraph rendering should be skipped for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's rendering option.</param>
        /// <returns><c>true</c> to skip paragraph rendering.</returns>
        public override bool IsTight(Block element, bool tight)
        {
            return element.OrderedList.IsTight;
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
                { "tight", element.OrderedList.IsTight },
                { "start", element.OrderedList.Start },
                { "delim", element.OrderedList.DelimiterCharacter },
            };
        }
    }

    internal sealed class LegacyOrderedListFormatter : OrderedListFormatter
    {
        public LegacyOrderedListFormatter(FormatterParameters parameters)
#pragma warning disable 0618
            : base(parameters, BlockTag.List, "list")
#pragma warning restore 0618
        {
        }

        public override bool CanHandle(Block element)
        {
#pragma warning disable 0618
            return element.Tag == BlockTag.List && element.ListData.ListType == ListType.Ordered;
#pragma warning restore 0618
        }

        public override IEnumerable<KeyValuePair<string, object>> GetSyntaxData(ISyntaxFormatter formatter, Block element)
        {
            return new Dictionary<string, object>
            {
                { "type", "ordered" },
#pragma warning disable 0618
                { "tight", element.ListData.IsTight },
                { "start", element.ListData.Start },
                { "delim", element.ListData.Delimiter },
#pragma warning restore 0618
            };
        }
    }
}
