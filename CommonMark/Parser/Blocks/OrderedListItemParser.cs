using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Ordered list item element parser.
    /// </summary>
    public class OrderedListItemParser : ListItemParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="first">The first in the range of the list marker characters.</param>
        /// <param name="last">The first in the range of the list marker characters.</param>
        public OrderedListItemParser(CommonMarkSettings settings, char first, char last)
            : base(settings, BlockTag.ListItem, BlockTag.List, ListType.Ordered, CreateCharacters(first, last))
        {
            First = first;
            Last = last;
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
        /// Attempts to parse an ordered list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">List data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        private int ParseMarker(BlockParserInfo info, out ListData data)
        {
            data = null;

            var c = info.CurrentCharacter;

            if (c < First || c > Last)
                return 0;

            int start = c - First;
            var ln = info.Line;
            var len = ln.Length;
            var pos = info.FirstNonspace;

            while (pos < len - 1)
            {
                c = ln[++pos];
                // We limit to 9 digits to avoid overflow, This also seems to be the limit for 'start' in some browsers. 
                if (c >= First && c <= Last && start < 100000000)
                    start = start * 10 + (c - First);
                else
                    break;
            }

            if (pos >= len - 1 || (c != '.' && c != ')'))
                return 0;

            return CompleteParseMarker(pos, info, '\0', c, start, out data);
        }

        private static char[] CreateCharacters(char first, char last)
        {
            var chars = new char[last - first + 1];
            for (var c = first; c <= last; c++)
                chars[c - first] = c;
            return chars;
        }

        private char First
        {
            get;
        }

        private char Last
        {
            get;
        }
    }
}
