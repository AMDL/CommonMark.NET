﻿using CommonMark.Syntax;
using System.Globalization;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Ordered list item parameters.
    /// </summary>
    public sealed class OrderedListItemParameters : ListItemParameters<ListItemDelimiterParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParameters"/> class.
        /// </summary>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="markerFirst">First marker character.</param>
        /// <param name="markerLast">Last marker character.</param>
        /// <param name="maxMarkerLength">Maximum marker length.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public OrderedListItemParameters(BlockTag parentTag, ListType listType, char markerFirst, char markerLast, int maxMarkerLength, params ListItemDelimiterParameters[] delimiters)
#pragma warning restore 0618
            : base(parentTag, listType, delimiters)
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
    /// Ordered list item element handler.
    /// </summary>
    public sealed class OrderedListItemHandler : ListItemHandler<OrderedListData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">List item element tag.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        public OrderedListItemHandler(CommonMarkSettings settings, BlockTag tag, char character, OrderedListItemParameters parameters)
#pragma warning disable 0618
            : base(settings, tag, character, parameters, parameters.Delimiters)
#pragma warning restore 0618
        {
            MarkerFirst = parameters.MarkerFirst;
            MarkerLast = parameters.MarkerLast;
            MaxMarkerLength = parameters.MaxMarkerLength;
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, IsListsMatch, SetListData);
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
        /// Attempts to parse an ordered list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Ordered list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected override int ParseMarker(BlockParserInfo info, out ListData data, out OrderedListData listData)
        {
            data = null;
            listData = null;

            var curChar = info.CurrentCharacter;
            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            do
                curChar = line[++offset];
            while (offset < length - 1 && curChar >= MarkerFirst && curChar <= MarkerLast);

            if (offset == length - 1)
                return 0;

            var startLength = offset - info.FirstNonspace;
            if (startLength > MaxMarkerLength)
                return 0;

            var startStr = line.Substring(info.FirstNonspace, startLength);

            return CompleteScan(info, offset, startStr, curChar, '\0', curChar,
                GetStart, GetListData, out data, out listData);
        }

        /// <summary>
        /// Calculates an integer start value.
        /// </summary>
        /// <param name="startStr">Start string.</param>
        /// <returns>Integer start value, or 1 if unsuccessful.</returns>
        protected override int GetStart(string startStr)
        {
            int start;
            if (!int.TryParse(startStr, out start))
                start = 1;
            return start;
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
            };

            return orderedListData;
        }

        private char MarkerFirst
        {
            get;
        }

        private char MarkerLast
        {
            get;
        }

        private int MaxMarkerLength
        {
            get;
        }
    }
}
