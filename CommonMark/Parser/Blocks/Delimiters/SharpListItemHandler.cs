using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Sharp decimal ordered list item delimiter handler.
    /// </summary>
    public sealed class SharpListItemHandler : OrderedListItemHandler<SharpListItemHandler.Parameters>
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly OrderedListItemParameters DefaultParameters = new OrderedListItemParameters(
            markers: new[]
            {
                new OrderedListSingleMarkerParameters('#', 0)
            },
            delimiters: new[]
            {
                new ListItemDelimiterParameters('.')
            },
            markerType: OrderedListMarkerType.Decimal);

        /// <summary>
        /// Handler parameters.
        /// </summary>
        public new sealed class Parameters : OrderedListItemHandler<Parameters>.Parameters
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Parameters"/> class.
            /// </summary>
            /// <param name="parameters">List item parameters.</param>
            public Parameters(OrderedListItemParameters parameters)
                : base(parameters, new[] { '#' }, false)
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        public SharpListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
            : base(settings, GetHandlerParameters(parameters))
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
        protected override bool AdjustStart(ref int start, ref int value, char curChar)
        {
            start = 1;
            return true;
        }

        private static Parameters GetHandlerParameters(OrderedListItemParameters parameters)
        {
            return new Parameters(parameters);
        }
    }
}
