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
        /// Creates a numeric ordered list item delimiter handler.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <returns>Delegate delimiter handler.</returns>
        public static IBlockDelimiterHandler Create(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            var range = parameters.Markers[0] as OrderedListMarkerRangeParameters;
            var characters = GetCharacters(range);
            var handlers = GetHandlers(settings, parameters);
            return DelegateBlockDelimiterHandler.Merge(characters, handlers);
        }

        private static IBlockDelimiterHandler[] GetHandlers(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            var length = parameters.Delimiters.Length;
            var handlers = new IBlockDelimiterHandler[length];
            for (var i = 0; i < length; i++)
                handlers[i] = new NumericListItemHandler(settings, parameters, parameters.Delimiters[i]);
            return handlers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public NumericListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
            : base(settings, parameters, delimiter, false)
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
