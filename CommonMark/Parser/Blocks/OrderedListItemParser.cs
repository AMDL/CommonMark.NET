using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Base ordered list marker parameters class.
    /// </summary>
    public abstract class OrderedListMarkerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListMarkerParameters"/> class.
        /// </summary>
        /// <param name="startValue">Start value.</param>
        protected OrderedListMarkerParameters(int startValue)
        {
            this.StartValue = startValue;
        }

        /// <summary>
        /// Gets or sets the start value (0 for decimal numerals).
        /// </summary>
        public int StartValue { get; set; }
    }

    /// <summary>
    /// Ordered list single marker parameters.
    /// </summary>
    class OrderedListSingleMarkerParameters : OrderedListMarkerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListSingleMarkerParameters"/> class.
        /// </summary>
        /// <param name="character">Marker character.</param>
        /// <param name="value">Value.</param>
        public OrderedListSingleMarkerParameters(char character, int value)
            : base(value)
        {
            this.Character = character;
        }

        /// <summary>
        /// Gets or sets the marker character.
        /// </summary>
        public char Character { get; set; }
    }

    /// <summary>
    /// Ordered list marker range parameters.
    /// </summary>
    public sealed class OrderedListMarkerRangeParameters : OrderedListMarkerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListMarkerRangeParameters"/> class.
        /// </summary>
        /// <param name="minChar">First marker character.</param>
        /// <param name="maxChar">Last marker character.</param>
        /// <param name="startValue">Start value.</param>
        /// <param name="valueBase">Value base.</param>
        public OrderedListMarkerRangeParameters(char minChar, char maxChar, int startValue, int valueBase)
            : base(startValue)
        {
            this.MinCharacter = minChar;
            this.MaxCharacter = maxChar;
            this.ValueBase = valueBase;
        }

        /// <summary>
        /// Gets or sets the first character in the list marker character range.
        /// </summary>
        public char MinCharacter { get; set; }

        /// <summary>
        /// Gets or sets the last character in the list marker character range.
        /// </summary>
        public char MaxCharacter { get; set; }

        /// <summary>
        /// Gets or sets the value base (10 for decimal numerals).
        /// </summary>
        public int ValueBase { get; set; }
    }

    /// <summary>
    /// Ordered list item parameters.
    /// </summary>
    public sealed class OrderedListItemParameters : ListItemParameters<ListItemDelimiterParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParameters"/> class.
        /// </summary>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="startValue">Start value (0 for decimal numerals).</param>
        /// <param name="valueBase">Value base (10 for decimal numerals).</param>
        /// <param name="maxMarkerLength">Maximum marker length.</param>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="markerType">Marker type.</param>
        /// <param name="listStyle">List style.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public OrderedListItemParameters(char markerMinChar, char markerMaxChar, int startValue = 0, int valueBase = 10, int maxMarkerLength = 9,
            BlockTag tag = BlockTag.ListItem, BlockTag parentTag = BlockTag.OrderedList, ListType listType = ListType.Ordered,
            OrderedListMarkerType markerType = OrderedListMarkerType.None, string listStyle = null, params ListItemDelimiterParameters[] delimiters)
#pragma warning restore 0618
            : this(new[] { new OrderedListMarkerRangeParameters(markerMinChar, markerMaxChar, startValue, valueBase) }, maxMarkerLength, tag, parentTag, listType, markerType, listStyle, delimiters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParameters"/> class.
        /// </summary>
        /// <param name="markers">Marker parameters.</param>
        /// <param name="maxMarkerLength">Maximum marker length.</param>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="markerType">Marker type.</param>
        /// <param name="listStyle">List style.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public OrderedListItemParameters(OrderedListMarkerParameters[] markers, int maxMarkerLength = 9,
            BlockTag tag = BlockTag.ListItem, BlockTag parentTag = BlockTag.OrderedList, ListType listType = ListType.Ordered,
            OrderedListMarkerType markerType = OrderedListMarkerType.None, string listStyle = null, params ListItemDelimiterParameters[] delimiters)
            : base(tag, parentTag, listType, delimiters)
#pragma warning restore 0618
        {
            this.MaxMarkerLength = maxMarkerLength;
            this.MarkerType = markerType;
            this.Markers = markers;
            this.ListStyle = listStyle;
        }

        /// <summary>
        /// Gets or sets the list marker parameters.
        /// </summary>
        public OrderedListMarkerParameters[] Markers { get; set; }

        /// <summary>
        /// Gets or sets the maximum marker length.
        /// </summary>
        public int MaxMarkerLength { get; set; }

        /// <summary>
        /// Gets or sets the list marker type.
        /// </summary>
        public OrderedListMarkerType MarkerType { get; set; }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        public string ListStyle { get; set; }
    }

    /// <summary>
    /// Base ordered list item delimiter handler class.
    /// </summary>
    public abstract class OrderedListItemHandler : ListItemHandler<OrderedListData>
    {
        /// <summary>
        /// Creates ordered list item handlers using the specified parameters.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <returns>A collection of ordered list item delimiter handlers.</returns>
        public static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.Markers.Length == 1)
                {
                    var handlers = SinleRangeOrderedListItemHandler.Create(settings, parameters, parameters.Markers[0] as OrderedListMarkerRangeParameters);
                    if (handlers != null && handlers.GetEnumerator().MoveNext())
                        return handlers;
                }
                return AlphaListItemHandler.Create(settings, parameters, parameters.Markers);
            }
            return new IBlockDelimiterHandler[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="isRequireContent"><c>true</c> if items on this list require content.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        public OrderedListItemHandler(CommonMarkSettings settings, char character, char markerMinChar, char markerMaxChar, bool isRequireContent, OrderedListItemParameters parameters)
            : base(settings, character, isRequireContent, parameters, parameters.Delimiters)
        {
            MarkerMinCharacter = markerMinChar;
            MarkerMaxCharacter = markerMaxChar;
            MaxMarkerLength = parameters.MaxMarkerLength;
            MarkerType = parameters.MarkerType;
            ListStyle = parameters.ListStyle;
        }

        /// <summary>
        /// Attempts to parse an ordered list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="adjustStart">Start value adjuster delegate.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Ordered list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected sealed override int ParseMarker(BlockParserInfo info, AdjustStartDelegate adjustStart, out ListData data, out OrderedListData listData)
        {
            data = null;
            listData = null;

            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            var curChar = info.CurrentCharacter;
            var start = 0;
            var value = 0;
            adjustStart(ref start, ref value, curChar);

            do
            {
                if (++offset == length - 1)
                    return 0; //Assuming MinSpaceCount > 0

                curChar = line[offset];
                if (curChar < MarkerMinCharacter || curChar > MarkerMaxCharacter)
                    break;

                if (!adjustStart(ref start, ref value, curChar))
                    return 0;
            }
            while (true);

            var markerLength = offset - info.FirstNonspace;
            if (markerLength > MaxMarkerLength)
                return 0;

            return CompleteScan(info, offset, start, curChar, '\0', curChar, GetListData, out data, out listData);
        }

        /// <summary>
        /// Determines whether a list item belongs to a matching ordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Ordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected override bool IsListsMatch(BlockParserInfo info, OrderedListData listData)
        {
            return listData.Equals(info.Container.OrderedListData);
        }

        /// <summary>
        /// Updates a container with ordered list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Ordered list data.</param>
        protected override void SetListData(BlockParserInfo info, OrderedListData listData)
        {
            info.Container.OrderedListData = listData;
        }

        /// <summary>
        /// Creates and initializes ordered list data.
        /// </summary>
        /// <param name="curChar">Current character.</param>
        /// <param name="start">Start value.</param>
        /// <returns></returns>
        protected override OrderedListData GetListData(char curChar, int start)
        {
            var orderedListData = new OrderedListData
            {
                Start = start,
                DelimiterCharacter = curChar,
                MarkerType = MarkerType,
                ListStyle = ListStyle,
            };

            return orderedListData;
        }

        /// <summary>
        /// Adjust the start value.
        /// </summary>
        /// <param name="start">Current start value.</param>
        /// <param name="value">Current character value.</param>
        /// <param name="curChar">Current character.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected abstract bool AdjustStart(ref int start, ref int value, char curChar);

        /// <summary>
        /// Gets the first marker character.
        /// </summary>
        protected char MarkerMinCharacter
        {
            get;
        }

        /// <summary>
        /// Gets the last marker character.
        /// </summary>
        protected char MarkerMaxCharacter
        {
            get;
        }

        /// <summary>
        /// Gets the maximum marker length.
        /// </summary>
        protected int MaxMarkerLength
        {
            get;
        }

        private OrderedListMarkerType MarkerType
        {
            get;
        }

        private string ListStyle
        {
            get;
        }
    }

    /// <summary>
    /// Single-range multiplicative ordered list item delimiter handler.
    /// </summary>
    public sealed class SinleRangeOrderedListItemHandler : OrderedListItemHandler
    {
        internal static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters, OrderedListMarkerRangeParameters range)
        {
            if (range != null)
            {
                for (var i = 0; i <= range.MaxCharacter - range.MinCharacter; i++)
                {
                    yield return new SinleRangeOrderedListItemHandler(settings, (char)(i + range.MinCharacter), range, parameters);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SinleRangeOrderedListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="range">Marker range parameters.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        public SinleRangeOrderedListItemHandler(CommonMarkSettings settings, char character, OrderedListMarkerRangeParameters range, OrderedListItemParameters parameters)
            : base(settings, character, range.MinCharacter, range.MaxCharacter, false, parameters)
        {
            StartValue = range.StartValue;
            ValueBase = range.ValueBase;
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
            value = curChar - MarkerMinCharacter + StartValue;
            start = start * ValueBase + value;
            return true;
        }

        private int StartValue
        {
            get;
        }

        private int ValueBase
        {
            get;
        }
    }

    /// <summary>
    /// Base additive ordered list item delimiter handler class.
    /// </summary>
    public abstract class AdditiveListItemHandler : OrderedListItemHandler
    {
        /// <summary>
        /// Creates a mapping from character to value.
        /// </summary>
        /// <param name="markers">Marker parameters.</param>
        /// <param name="valueMapDict">Value map dictionary.</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <returns></returns>
        protected static int[] CreateValueMap(OrderedListMarkerParameters[] markers, Dictionary<char, int> valueMapDict, out char markerMinChar, out char markerMaxChar)
        {
            markerMinChar = char.MaxValue;
            markerMaxChar = char.MinValue;
            foreach (var marker in markers)
            {
                if (!AddSingle(marker as OrderedListSingleMarkerParameters, valueMapDict, ref markerMinChar, ref markerMaxChar)
                    && !AddRange(marker as OrderedListMarkerRangeParameters, valueMapDict, ref markerMinChar, ref markerMaxChar))
                {
                    throw new System.InvalidOperationException();
                }
            }

            var valueMap = new int[markerMaxChar - markerMinChar + 1];
            foreach (var kvp in valueMapDict)
            {
                valueMap[kvp.Key - markerMinChar] = kvp.Value;
            }

            return valueMap;
        }

        private static bool AddSingle(OrderedListSingleMarkerParameters single, Dictionary<char, int> valueMap, ref char min, ref char max)
        {
            if (single == null)
                return false;

            var singleChar = single.Character;
            valueMap.Add(singleChar, single.StartValue);
            if (singleChar < min)
                min = singleChar;
            if (singleChar > max)
                max = singleChar;

            return true;
        }

        private static bool AddRange(OrderedListMarkerRangeParameters range, Dictionary<char, int> valueMap, ref char min, ref char max)
        {
            if (range == null)
                return false;

            var rangeMin = range.MinCharacter;
            var rangeMax = range.MaxCharacter;
            for (int i = 0; i <= rangeMax - rangeMin; i++)
            {
                valueMap.Add((char)(i + rangeMin), range.StartValue + i);
            }
            if (rangeMin < min)
                min = rangeMin;
            if (rangeMax > max)
                max = rangeMax;

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="valueMap">Character to value mapping (<paramref name="markerMinChar"/>-based).</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        public AdditiveListItemHandler(CommonMarkSettings settings, char character, int[] valueMap, char markerMinChar, char markerMaxChar, OrderedListItemParameters parameters)
            : base(settings, character, markerMinChar, markerMaxChar, true, parameters)
        {
            this.ValueMap = valueMap;
        }

        /// <summary>
        /// Gets the value map.
        /// </summary>
        protected int[] ValueMap
        {
            get;
        }
    }

    /// <summary>
    /// Alphabetical ordered list item delimiter handler.
    /// </summary>
    public sealed class AlphaListItemHandler : AdditiveListItemHandler
    {
        internal static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters, OrderedListMarkerParameters[] markers)
        {
            char min;
            char max;
            var valueMapDict = new Dictionary<char, int>();
            var valueMap = CreateValueMap(markers, valueMapDict, out min, out max);

            foreach (var kvp in valueMapDict)
            {
                yield return new AlphaListItemHandler(settings, kvp.Key, valueMap, min, max, parameters);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="valueMap">Character to value mapping (<paramref name="markerMinChar"/>-based).</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        public AlphaListItemHandler(CommonMarkSettings settings, char character, int[] valueMap, char markerMinChar, char markerMaxChar, OrderedListItemParameters parameters)
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
            if ((value = ValueMap[curChar - MarkerMinCharacter]) == 0)
                return false;
            start += value;
            return true;
        }
    }

    /// <summary>
    /// Roman numeral ordered list item delimiter handler.
    /// </summary>
    public sealed class RomanNumeralListItemHandler : AdditiveListItemHandler
    {
        internal static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters, OrderedListMarkerParameters[] markers)
        {
            char min;
            char max;
            var valueMapDict = new Dictionary<char, int>();
            var valueMap = CreateValueMap(markers, valueMapDict, out min, out max);

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
    }
}
