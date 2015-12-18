using CommonMark.Syntax;
using System.Globalization;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Ordered list parameters.
    /// </summary>
    public sealed class OrderedListParameters : ListParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListParameters"/> class.
        /// </summary>
        /// <param name="delimiters">Delimiter parameters.</param>
        /// <param name="markerFirst">First marker character.</param>
        /// <param name="markerLast">Last marker character.</param>
        /// <param name="maxMarkerLength">Maximum marker length.</param>
        public OrderedListParameters(ListItemDelimiterParameters[] delimiters, char markerFirst, char markerLast, int maxMarkerLength)
            : base(delimiters)
        {
            this.MarkerFirst = markerFirst;
            this.MarkerLast = markerLast;
            this.MaxMarkerLength = maxMarkerLength;
        }

        /// <summary>
        /// Gets or sets the first character in the list marker character range.
        /// </summary>
        public char MarkerFirst { get; set; }

        /// <summary>
        /// Gets or sets the last character in the list marker character range.
        /// </summary>
        public char MarkerLast { get; set; }

        /// <summary>
        /// Gets or sets the maximum marker length.
        /// </summary>
        public int MaxMarkerLength { get; set; }
    }

    /// <summary>
    /// Ordered list item element parser.
    /// </summary>
    public abstract class OrderedListItemParser : ListItemParser<OrderedListData, OrderedListParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="parameters">List parameters.</param>
        public OrderedListItemParser(CommonMarkSettings settings, BlockTag parentTag, OrderedListParameters parameters)
#pragma warning disable 0618
            : base(settings, BlockTag.ListItem, parentTag, ListType.Ordered, GetCharacters(parameters), parameters)
#pragma warning restore 0618
        {
        }

        /// <summary>
        /// Determines whether a list item belongs to an ordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="orderedListData">Ordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="orderedListData"/>.</returns>
        protected override bool IsListsMatch(BlockParserInfo info, OrderedListData orderedListData)
        {
            return orderedListData.Equals(info.Container.OrderedListData);
        }

        /// <summary>
        /// Updates a container with ordered list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="orderedListData">Ordered list data.</param>
        protected override void SetListData(BlockParserInfo info, OrderedListData orderedListData)
        {
            info.Container.OrderedListData = orderedListData;
        }

        /// <summary>
        /// Attempts to parse an ordered list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="orderedListData">Ordered list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected override int ParseMarker(BlockParserInfo info, out ListData data, out OrderedListData orderedListData)
        {
            data = null;
            orderedListData = null;

            var curChar = info.CurrentCharacter;
            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            //TODO Remove after extracting opener
            if (curChar < MarkerFirst || curChar > MarkerLast)
                return 0;

            do
                curChar = line[++offset];
            while (offset < length - 1 && curChar >= MarkerFirst && curChar <= MarkerLast);

            if (offset == length - 1)
                return 0;

            var startLength = offset - info.FirstNonspace;
            if (startLength > MaxMarkerLength)
                return 0;

            var start = line.Substring(info.FirstNonspace, startLength);

            return CompleteScan(info, offset, start, curChar, '\0', curChar,
                GetIntStart, GetOrderedListData, out data, out orderedListData);
        }

        private static int GetIntStart(string start)
        {
            int intStart;
            if (!int.TryParse(start, NumberStyles.None, CultureInfo.InvariantCulture, out intStart))
                intStart = 1;
            return intStart;
        }

        private static OrderedListData GetOrderedListData(char curChar, string start, int intStart)
        {
            var orderedListData = new OrderedListData
            {
                Start = start,
                DelimiterCharacter = curChar,
            };

            if (intStart != 1)
                orderedListData.Start = intStart.ToString(CultureInfo.InvariantCulture);

            return orderedListData;
        }

        private static char[] GetCharacters(OrderedListParameters parameters)
        {
            var length = parameters.MarkerLast - parameters.MarkerFirst + 1;
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = (char)(i + parameters.MarkerFirst);
            return chars;
        }

        private char MarkerFirst
        {
            get { return Parameters.MarkerFirst; }
        }

        private char MarkerLast
        {
            get { return Parameters.MarkerLast; }
        }

        private int MaxMarkerLength
        {
            get { return Parameters.MaxMarkerLength; }
        }
    }
}
