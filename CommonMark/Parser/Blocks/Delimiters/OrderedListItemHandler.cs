﻿using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Base ordered list item delimiter handler class.
    /// </summary>
    public abstract class OrderedListItemHandler<TParameters> : ListItemHandler<OrderedListData, OrderedListItemParameters, TParameters>
        where TParameters : OrderedListItemHandler<TParameters>.Parameters
    {
        /// <summary>
        /// Handler parameters.
        /// </summary>
        public abstract class Parameters : ListItemHandlerParameters<OrderedListItemParameters>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Parameters"/> class.
            /// </summary>
            /// <param name="parameters">List item parameters.</param>
            /// <param name="characters">Handled characters.</param>
            /// <param name="isRequireContent"><c>true</c> if items on this list require content.</param>
            protected Parameters(OrderedListItemParameters parameters, char[] characters, bool isRequireContent)
                : base(parameters, characters, parameters.Delimiters, isRequireContent)
            {
            }

            /// <summary>
            /// Gets or sets the first marker character.
            /// </summary>
            public char MarkerMinChar { get; set; }

            /// <summary>
            /// Gets or sets the last marker character.
            /// </summary>
            public char MarkerMaxChar { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemHandler{TParameters}"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="handlerParameters">Handler parameters.</param>
        public OrderedListItemHandler(CommonMarkSettings settings, TParameters handlerParameters)
            : base(settings, handlerParameters)
        {
            MarkerMinCharacter = handlerParameters.MarkerMinChar;
            MarkerMaxCharacter = handlerParameters.MarkerMaxChar;
            MaxMarkerLength = handlerParameters.Parameters.MaxMarkerLength;
            MarkerType = handlerParameters.Parameters.MarkerType;
            ListStyle = handlerParameters.Parameters.ListStyle;
            ValueBase = handlerParameters.Parameters.ValueBase;
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

        /// <summary>
        /// Gets the value base.
        /// </summary>
        protected int ValueBase
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
}