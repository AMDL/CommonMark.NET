using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list item element parser.
    /// </summary>
    public class BulletListItemParser : ListItemParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="isHorizontalRuleMarkers"><c>true</c> if <paramref name="markers"/> double as horizontal rule markers.</param>
        /// <param name="markers">List markers.</param>
        public BulletListItemParser(CommonMarkSettings settings, bool isHorizontalRuleMarkers, params char[] markers)
            : base(settings, BlockTag.ListItem, BlockTag.List, ListType.Bullet, markers)
        {
            IsHorizontalRuleMarkers = isHorizontalRuleMarkers;
        }


        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            int markerLength;
            ListData data;
            if (!CanOpen(info) || 0 == (markerLength = ParseMarker(info, out data)))
                return false;
            return DoOpen(info, markerLength, data);
        }

        /// <summary>
        /// Attempts to parse a bullet list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">List data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        private int ParseMarker(BlockParserInfo info, out ListData data)
        {
            data = null;

            var c = info.CurrentCharacter;

            if (System.Array.IndexOf(Characters, c) < 0 || (IsHorizontalRuleMarkers && ScanHorizontalRule(info, c)))
                return 0;

            return CompleteParseMarker(info.FirstNonspace, info, c, '\0', 1, out data);
        }

        private bool IsHorizontalRuleMarkers
        {
            get;
        }
    }
}
