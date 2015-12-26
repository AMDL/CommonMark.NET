﻿using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Roman numeral ordered list item delimiter handler.
    /// </summary>
    public sealed class RomanListItemHandler : MappingListItemHandler
    {
        /// <summary>
        /// The default parameters for lowercase Roman numeral lists.
        /// </summary>
        public static readonly OrderedListItemParameters LowerRomanParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.LowerRoman,
            listStyle: "lower-roman",
            markers: new[]
            {
                new OrderedListSingleMarkerParameters('i', value:1),
                new OrderedListSingleMarkerParameters('v', value:5),
                new OrderedListSingleMarkerParameters('x', value:10),
                new OrderedListSingleMarkerParameters('l', value:50),
                new OrderedListSingleMarkerParameters('c', value:100),
                new OrderedListSingleMarkerParameters('m', value:1000),
            },
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.', 1, 9),
                new ListItemDelimiterParameters(')', 1, 3),
            });

        /// <summary>
        /// The default parameters for uppercase Roman numeral lists.
        /// </summary>
        public static readonly OrderedListItemParameters UpperRomanParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.UpperRoman,
            listStyle: "upper-roman",
            markers: new[]
            {
                new OrderedListSingleMarkerParameters('I', value:1),
                new OrderedListSingleMarkerParameters('V', value:5),
                new OrderedListSingleMarkerParameters('X', value:10),
                new OrderedListSingleMarkerParameters('L', value:50),
                new OrderedListSingleMarkerParameters('C', value:100),
                new OrderedListSingleMarkerParameters('M', value:1000),
            },
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.', 2, 9),
                new ListItemDelimiterParameters(')', 1, 3),
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="RomanListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public RomanListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
            : base(settings, parameters, delimiter)
        {
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, AdjustStart, MatchList, SetListData);
        }

        /// <summary>
        /// Adjust the start value.
        /// </summary>
        /// <param name="start">Current start value.</param>
        /// <param name="value">Current character value.</param>
        /// <param name="curChar">Current character.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected override bool AdjustStart(ref int start, ref short value, char curChar)
        {
            var prevValue = value;

            if ((value = ValueMap[curChar - MarkerMinCharacter]) == 0)
                return false;

            if (start > 0)
            {
                if (prevValue < value)
                {
                    if (prevValue * 2 > value)
                        return false;
                    value -= (short)(prevValue * 2);
                }
                else if (value > start)
                    return false;
            }

            start += value;
            return true;
        }

        /// <summary>
        /// Matches a list item to a Roman numeral ordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Ordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected override bool MatchList(BlockParserInfo info, OrderedListData listData)
        {
            var containerListData = info.Container.OrderedListData;
            if (containerListData == null || containerListData.DelimiterCharacter != listData.DelimiterCharacter)
                return false;
            return containerListData.MarkerType == listData.MarkerType
                || containerListData.MarkerType == OrderedListMarkerType.LowerLatin && listData.MarkerType == OrderedListMarkerType.LowerRoman
                || containerListData.MarkerType == OrderedListMarkerType.UpperLatin && listData.MarkerType == OrderedListMarkerType.UpperRoman;
        }
    }
}
