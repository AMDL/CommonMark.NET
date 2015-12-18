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
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="listType">List type.</param>
        /// <param name="markers">List marker characters.</param>
        public ListItemParser(CommonMarkSettings settings, BlockTag tag, BlockTag parentTag, ListType listType, char[] markers)
            : base(settings, tag, markers)
        {
            ParentTag = parentTag;
            ListType = listType;
        }

        /// <summary>
        /// Gets the type of the parent list.
        /// </summary>
        public ListType ListType
        {
            get;
        }

        /// <summary>
        /// Gets the element tag of the parent list.
        /// </summary>
        public BlockTag ParentTag
        {
            get;
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
        /// Determines whether a list item can be opened.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the current line may contain a handled list item element.</returns>
        protected bool CanOpen(BlockParserInfo info)
        {
            return (!info.IsIndented || info.Container.Tag == ParentTag);
        }

        /// <summary>
        /// Opens a list item.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="markerLength">Marker length.</param>
        /// <param name="data">List data.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected bool DoOpen(BlockParserInfo info, int markerLength, ListData data)
        {
            // save the offset before advancing it
            data.MarkerOffset = info.Indent;

            // compute padding:
            info.AdvanceOffset(info.FirstNonspace + markerLength - info.Offset, false);
            var i = 0;
            while (i <= 5 && info.Line[info.Offset + i] == ' ')
                i++;

            // i = number of spaces after marker, up to 5
            if (i >= 5 || i < 1 || info.Line[info.Offset] == '\n')
            {
                data.Padding = markerLength + 1;
                if (i > 0)
                {
                    info.Column++;
                    info.Offset++;
                }
            }
            else
            {
                data.Padding = markerLength + i;
                info.AdvanceOffset(i, true);
            }

            // check container; if it's a list, see if this list item
            // can continue the list; otherwise, create a list container.

            if (info.Container.Tag != ParentTag || !ListsMatch(info.Container.ListData, data))
            {
                info.Container = CreateChildBlock(info, ParentTag, info.FirstNonspace);
                info.Container.ListData = data;
            }

            // add the list item
            info.Container = CreateChildBlock(info, Tag, info.FirstNonspace);
            info.Container.ListData = data;

            return true;
        }

        private static bool ListsMatch(ListData listData, ListData itemData)
        {
            return (listData.ListType == itemData.ListType &&
                    listData.Delimiter == itemData.Delimiter &&
                // list_data.marker_offset == item_data.marker_offset &&
                    listData.BulletChar == itemData.BulletChar);
        }

        /// <summary>
        /// Completes the list marker parsing.
        /// </summary>
        /// <param name="pos">Current position.</param>
        /// <param name="info">Parser state.</param>
        /// <param name="bulletChar">Bullet character.</param>
        /// <param name="delimiter">Marker delimiter character.</param>
        /// <param name="start">Start value.</param>
        /// <param name="data">List data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected int CompleteParseMarker(int pos, BlockParserInfo info, char bulletChar, char delimiter, int start, out ListData data)
        {
            pos++;

            if (pos == info.Line.Length || (info.Line[pos] != ' ' && info.Line[pos] != '\n'))
            {
                data = null;
                return 0;
            }

            data = new ListData
            {
                ListType = ListType,
                BulletChar = bulletChar,
                Start = start,
                Delimiter = (delimiter == '.' ? ListDelimiter.Period : ListDelimiter.Parenthesis)
            };

            return pos - info.FirstNonspace;
        }
    }
}
