using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// ASCII letter ordered list item delimiter handler.
    /// </summary>
    public sealed class LatinListItemHandler : SingleRangeListItemHandler
    {
        /// <summary>
        /// The default parameters for lowercase ASCII letter lists.
        /// </summary>
        public static readonly OrderedListItemParameters LowerLatinParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.LowerLatin,
            listStyle: "lower-latin",
            markerMinChar: 'a',
            markerMaxChar: 'z',
            startValue: 1,
            delimiters: ListItemDelimiterParameters.DefaultLower);

        /// <summary>
        /// The default parameters for uppercase ASCII letter lists.
        /// </summary>
        public static readonly OrderedListItemParameters UpperLatinParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.UpperLatin,
            listStyle: "upper-latin",
            markerMinChar: 'A',
            markerMaxChar: 'Z',
            startValue: 1,
            delimiters: ListItemDelimiterParameters.DefaultUpper);

        /// <summary>
        /// Initializes a new instance of the <see cref="LatinListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public LatinListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
            : base(settings, parameters, delimiter, true)
        {
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, AdjustStart, MatchList, SetList);
        }

        /// <summary>
        /// Matches a list item to a ASCII letter ordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="list">Ordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="list"/>.</returns>
        protected override bool MatchList(BlockParserInfo info, OrderedListData list)
        {
            var containerList = info.Container.OrderedList;
            if (containerList == null || containerList.DelimiterCharacter != list.DelimiterCharacter)
                return false;
            if (containerList.MarkerType == list.MarkerType)
                return true;
            if (containerList.MarkerType == OrderedListMarkerType.LowerRoman && list.MarkerType == OrderedListMarkerType.LowerLatin
                || containerList.MarkerType == OrderedListMarkerType.UpperRoman && list.MarkerType == OrderedListMarkerType.UpperLatin)
            {
                int start = 0;
                short value = 0;
                AdjustStart(ref start, ref value, info.CurrentCharacter);
                containerList.Start = start - 1; //assuming consecutive markers
                containerList.MarkerType = list.MarkerType;
                containerList.Style = list.Style;
                return true;
            }
            return false;
        }
    }
}
