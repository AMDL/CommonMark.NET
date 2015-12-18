using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.ListItem"/> element parser.
    /// </summary>
    public class ListItemParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public ListItemParser(CommonMarkSettings settings)
            : base(settings, BlockTag.ListItem, '+', '•', '*', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9')
        {
        }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if a blank line at the end of the handled element should be discarded.</returns>
        public override bool IsDiscardLastBlank(BlockParserInfo info)
        {
            // we don't set IsLastLineBlank on an empty list item.
            return info.Container.FirstChild == null
                && info.Container.SourcePosition >= info.LineInfo.LineOffset;
        }

        /// <summary>
        /// Determines whether a handled element can contain child elements of the specified kind.
        /// </summary>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if handled elements can contain elements having <paramref name="childTag"/>.</returns>
        public override bool CanContain(BlockTag childTag)
        {
            return true;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            if (info.Indent >= info.Container.ListData.MarkerOffset + info.Container.ListData.Padding)
            {
                info.AdvanceOffset(info.Container.ListData.MarkerOffset + info.Container.ListData.Padding, true);
                return true;
            }
            if (info.IsBlank && info.Container.FirstChild != null)
            {
                // if container->first_child is NULL, then the opening line
                // of the list item was blank after the list marker; in this
                // case, we are done with the list item.
                info.AdvanceOffset(info.FirstNonspace - info.Offset, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            int indent = info.Indent;
            int matched = 0;
            ListData data;
            if ((!info.IsIndented || info.Container.Tag == BlockTag.List) && 0 != (matched = ParseListMarker(info, out data)))
            {
                // compute padding:
                info.AdvanceOffset(info.FirstNonspace + matched - info.Offset, false);
                var i = 0;
                while (i <= 5 && info.Line[info.Offset + i] == ' ')
                    i++;

                // i = number of spaces after marker, up to 5
                if (i >= 5 || i < 1 || info.Line[info.Offset] == '\n')
                {
                    data.Padding = matched + 1;
                    if (i > 0)
                    {
                        info.Column++;
                        info.Offset++;
                    }
                }
                else
                {
                    data.Padding = matched + i;
                    info.AdvanceOffset(i, true);
                }

                // check container; if it's a list, see if this list item
                // can continue the list; otherwise, create a list container.

                data.MarkerOffset = indent;

                if (info.Container.Tag != BlockTag.List || !ListsMatch(info.Container.ListData, data))
                {
                    info.Container = CreateChildBlock(info, BlockTag.List, info.FirstNonspace);
                    info.Container.ListData = data;
                }

                // add the list item
                info.Container = CreateChildBlock(info, BlockTag.ListItem, info.FirstNonspace);
                info.Container.ListData = data;
            }

            return matched > 0;
        }

        private static bool ListsMatch(ListData listData, ListData itemData)
        {
            return (listData.ListType == itemData.ListType &&
                    listData.Delimiter == itemData.Delimiter &&
                // list_data.marker_offset == item_data.marker_offset &&
                    listData.BulletChar == itemData.BulletChar);
        }

        /// <summary>
        /// Attempts to parse a list item marker (bullet or enumerated).
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">List data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        private int ParseListMarker(BlockParserInfo info, out ListData data)
        {
            var ln = info.Line;
            var pos = info.FirstNonspace;

            data = null;
            var len = ln.Length;

            int startpos = pos;
            char c = ln[pos];

            if (c == '+' || c == '•' || (c == '*' && !ScanHorizontalRule(info, '*')) || (c == '-' && !ScanHorizontalRule(info, '-')))
            {
                pos++;
                if (pos == len || (ln[pos] != ' ' && ln[pos] != '\n'))
                    return 0;

                data = new ListData();
                data.BulletChar = c;
                data.Start = 1;
            }
            else if (c >= '0' && c <= '9')
            {

                int start = c - '0';

                while (pos < len - 1)
                {
                    c = ln[++pos];
                    // We limit to 9 digits to avoid overflow, This also seems to be the limit for 'start' in some browsers. 
                    if (c >= '0' && c <= '9' && start < 100000000)
                        start = start * 10 + (c - '0');
                    else
                        break;
                }

                if (pos >= len - 1 || (c != '.' && c != ')'))
                    return 0;

                pos++;
                if (pos == len || (ln[pos] != ' ' && ln[pos] != '\n'))
                    return 0;

                data = new ListData();
                data.ListType = ListType.Ordered;
                data.BulletChar = '\0';
                data.Start = start;
                data.Delimiter = (c == '.' ? ListDelimiter.Period : ListDelimiter.Parenthesis);

            }
            else
            {
                return 0;
            }

            return (pos - startpos);
        }
    }
}
