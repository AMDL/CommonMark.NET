using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// European numeral list item element parser.
    /// </summary>
    public sealed class EuropeanNumeralListItemParser : OrderedListItemParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly OrderedListParameters DefaultParameters = new OrderedListParameters(new[]
            {
                new ListItemDelimiterParameters('.'),
                new ListItemDelimiterParameters(')'),
            },
            markerFirst: '0',
            markerLast: '9',
            maxMarkerLength: 9); // We limit to 9 digits to avoid overflow, This also seems to be the limit for 'start' in some browsers. 

        /// <summary>
        /// Initializes a new instance of the <see cref="EuropeanNumeralListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="parameters">List parameters.</param>
        public EuropeanNumeralListItemParser(CommonMarkSettings settings, BlockTag parentTag, OrderedListParameters parameters)
            : base(settings, parentTag, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuropeanNumeralListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public EuropeanNumeralListItemParser(CommonMarkSettings settings)
            : this(settings, BlockTag.OrderedList, DefaultParameters)
        {
        }

        /// <summary>
        /// Opens a list item.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            return DoOpen(info, CanOpen, ParseMarker, IsListsMatch, SetListData);
        }
    }
}
