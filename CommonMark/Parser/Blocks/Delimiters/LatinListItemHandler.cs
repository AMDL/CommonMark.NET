using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// ASCII letter ordered list item delimiter handler.
    /// </summary>
    public sealed class LatinListItemHandler : MappingListItemHandler
    {
        /// <summary>
        /// The default parameters for lowercase ASCII letter lists.
        /// </summary>
        public static readonly OrderedListItemParameters LowerLatinParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.LowerLatin,
            listStyle: "lower-latin",
            markerMinChar: 'a',
            markerMaxChar: 'z',
            maxMarkerLength: 3,
            startValue: 1,
            valueBase: 26,
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.', 1),
                new ListItemDelimiterParameters(')', 1),
            });

        /// <summary>
        /// The default parameters for uppercase ASCII letter lists.
        /// </summary>
        public static readonly OrderedListItemParameters UpperLatinParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.UpperLatin,
            listStyle: "upper-latin",
            markerMinChar: 'A',
            markerMaxChar: 'Z',
            maxMarkerLength: 3,
            startValue: 1,
            valueBase: 26,
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.', 2),
                new ListItemDelimiterParameters(')', 1),
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="LatinListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        public LatinListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
            : base(settings, parameters)
        {
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, AdjustStart, IsListsMatch, SetListData);
        }

        /// <summary>
        /// Determines whether a list item belongs to a matching ordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Ordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected override bool IsListsMatch(BlockParserInfo info, OrderedListData listData)
        {
            if (base.IsListsMatch(info, listData))
                return true;
            var containerListData = info.Container.OrderedListData;
            if (containerListData != null && containerListData.DelimiterCharacter == listData.DelimiterCharacter
                && (containerListData.MarkerType == OrderedListMarkerType.LowerRoman && listData.MarkerType == OrderedListMarkerType.LowerLatin
                || containerListData.MarkerType == OrderedListMarkerType.UpperRoman && listData.MarkerType == OrderedListMarkerType.UpperLatin))
            {
                var start = 0;
                var value = 0;
                AdjustStart(ref start, ref value, info.CurrentCharacter);
                containerListData.Start = start - 1; //assuming consecutive markers
                containerListData.MarkerType = listData.MarkerType;
                containerListData.ListStyle = listData.ListStyle;
                return true;
            }
            return false;
        }
    }
}
