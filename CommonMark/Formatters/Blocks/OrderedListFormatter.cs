using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.OrderedList"/> element formatter.
    /// </summary>
    public class OrderedListFormatter : ListFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="printerTag">Printer tag.</param>
        public OrderedListFormatter(FormatterParameters parameters, BlockTag tag = BlockTag.OrderedList, string printerTag = "ordered_list")
            : base(parameters, tag, "ol", printerTag)
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
            writer.EnsureLine();
            var data = element.ListData;
            var orderedListData = element.OrderedListData;
            writer.WriteConstant("<ol");
            if (orderedListData.Start != "1")
            {
                writer.WriteConstant(" start=\"");
                writer.WriteConstant(orderedListData.Start);
                writer.Write('\"');
            }
            WritePosition(writer, element);
            writer.WriteLine('>');
            return true;
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { "tight", element.ListData.IsTight },
                { "start", element.OrderedListData.Start },
                { "delim", element.OrderedListData.DelimiterCharacter },
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

        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { "type", "ordered" },
                { "tight", element.ListData.IsTight },
#pragma warning disable 0618
                { "start", element.ListData.Start },
                { "delim", element.ListData.Delimiter },
#pragma warning restore 0618
            };
        }
    }
}
