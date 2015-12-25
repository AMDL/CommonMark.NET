namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Alphabetical ordered list item delimiter handler.
    /// </summary>
    public sealed class AlphaListItemHandler : MappingListItemHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public AlphaListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
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
    }
}
