namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Hash decimal ordered list item delimiter handler.
    /// </summary>
    public sealed class HashListItemHandler : OrderedListItemHandler<HashListItemHandler.Parameters>
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly OrderedListItemParameters DefaultParameters = new OrderedListItemParameters(
            markerChars: new[] { '#' },
            maxMarkerLength: 1);

        /// <summary>
        /// Handler parameters.
        /// </summary>
        public new sealed class Parameters : OrderedListItemHandler<Parameters>.Parameters
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Parameters"/> class.
            /// </summary>
            /// <param name="parameters">List item parameters.</param>
            /// <param name="delimiter">Delimiter parameters.</param>
            public Parameters(OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
                : base(parameters, new[] { '#' }, delimiter, false)
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public HashListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
            : base(settings, GetHandlerParameters(parameters, delimiter))
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
            start = 1;
            return true;
        }

        private static Parameters GetHandlerParameters(OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            return new Parameters(parameters, delimiter);
        }
    }
}
