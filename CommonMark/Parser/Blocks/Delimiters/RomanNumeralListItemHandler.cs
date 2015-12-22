using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Roman numeral ordered list item delimiter handler.
    /// </summary>
    public sealed class RomanNumeralListItemHandler : MappingListItemHandler
    {
        /// <summary>
        /// The default parameters for lowercase Roman numeral lists.
        /// </summary>
        public static readonly OrderedListItemParameters LowerRomanParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.LowerRoman,
            listStyle: "lower-roman",
            maxMarkerLength: 9,
            markers: new[]
            {
                new OrderedListSingleMarkerParameters('i', 1),
                new OrderedListSingleMarkerParameters('v', 5),
                new OrderedListSingleMarkerParameters('x', 10),
                new OrderedListSingleMarkerParameters('l', 50),
                new OrderedListSingleMarkerParameters('c', 100),
                new OrderedListSingleMarkerParameters('m', 1000),
            },
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.', 1),
                new ListItemDelimiterParameters(')', 1),
            });

        /// <summary>
        /// The default parameters for uppercase Roman numeral lists.
        /// </summary>
        public static readonly OrderedListItemParameters UpperRomanParameters = new OrderedListItemParameters(
            markerType: OrderedListMarkerType.UpperRoman,
            listStyle: "upper-roman",
            maxMarkerLength: 9,
            markers: new[]
            {
                new OrderedListSingleMarkerParameters('I', 1),
                new OrderedListSingleMarkerParameters('V', 5),
                new OrderedListSingleMarkerParameters('X', 10),
                new OrderedListSingleMarkerParameters('L', 50),
                new OrderedListSingleMarkerParameters('C', 100),
                new OrderedListSingleMarkerParameters('M', 1000),
            },
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.', 2),
                new ListItemDelimiterParameters(')', 1),
            });

        /// <summary>
        /// Creates Roman numeral list item delimiter handlers using the specified parameters.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <returns>A collection of Roman numeral list item delimiter handlers.</returns>
        public static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            char min;
            char max;
            var valueMapDict = new Dictionary<char, int>();
            var valueMap = CreateValueMap(parameters.Markers, valueMapDict, out min, out max);

            foreach (var kvp in valueMapDict)
            {
                yield return new RomanNumeralListItemHandler(settings, kvp.Key, valueMap, min, max, parameters);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RomanNumeralListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="valueMap">Character to value mapping (<paramref name="markerMinChar"/>-based).</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        public RomanNumeralListItemHandler(CommonMarkSettings settings, char character, int[] valueMap, char markerMinChar, char markerMaxChar, OrderedListItemParameters parameters)
            : base(settings, character, valueMap, markerMinChar, markerMaxChar, parameters)
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
        /// Adjust the start value.
        /// </summary>
        /// <param name="start">Current start value.</param>
        /// <param name="value">Current character value.</param>
        /// <param name="curChar">Current character.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected override bool AdjustStart(ref int start, ref int value, char curChar)
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
                    value -= prevValue * 2;
                }
                else if (value > start)
                    return false;
            }

            start += value;
            return true;
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
            return containerListData != null && containerListData.DelimiterCharacter == listData.DelimiterCharacter
                && (containerListData.MarkerType == OrderedListMarkerType.LowerLatin && listData.MarkerType == OrderedListMarkerType.LowerRoman
                || containerListData.MarkerType == OrderedListMarkerType.UpperLatin && listData.MarkerType == OrderedListMarkerType.UpperRoman);
        }
    }
}
