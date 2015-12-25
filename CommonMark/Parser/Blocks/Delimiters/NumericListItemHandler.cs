using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Numeric ordered list item delimiter handler.
    /// </summary>
    public sealed class NumericListItemHandler : SingleRangeListItemHandler
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly OrderedListItemParameters DefaultParameters = new OrderedListItemParameters(
            '0', '9', 0, 10, 9,
            BlockTag.ListItem,
            BlockTag.OrderedList,
#pragma warning disable 0618
            ListType.Ordered
#pragma warning restore 0618
        );

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        public NumericListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
            : base(settings, parameters, false)
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
    }
}
