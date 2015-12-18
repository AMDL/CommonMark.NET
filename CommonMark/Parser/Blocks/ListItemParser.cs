using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// List parameters.
    /// </summary>
    public abstract class ListParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListParameters"/> class.
        /// </summary>
        /// <param name="delimiters">Delimiter parameters.</param>
        protected ListParameters(ListItemDelimiterParameters[] delimiters)
        {
            this.Delimiters = delimiters;
        }

        /// <summary>
        /// Gets or sets the delimiter parameters.
        /// </summary>
        public ListItemDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.ListItem"/> element parser.
    /// </summary>
    /// <typeparam name="TData">Type of list data.</typeparam>
    /// <typeparam name="TParameters">Type of parameters.</typeparam>
    public abstract class ListItemParser<TData, TParameters> : BlockParser
        where TData : class
        where TParameters : ListParameters
    {
        /// <summary>
        /// List marker parser delegate.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected delegate int ParseMarkerDelegate(BlockParserInfo info, out ListData data, out TData listData);

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParser{TData,TParameters}"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="listType">List type.</param>
        /// <param name="markers">List marker characters.</param>
        /// <param name="parameters">Specific list parameters.</param>
#pragma warning disable 0618
        public ListItemParser(CommonMarkSettings settings, BlockTag tag, BlockTag parentTag, ListType listType, char[] markers, TParameters parameters)
#pragma warning restore 0618
            : base(settings, tag, markers)
        {
            ParentTag = parentTag;
            ListType = listType;
            Parameters = parameters;
            SetDelimiters(parameters.Delimiters);
        }

        /// <summary>
        /// Gets the type of the parent list.
        /// </summary>
#pragma warning disable 0618
        public ListType ListType
#pragma warning restore 0618
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
        /// Gets the list parameters.
        /// </summary>
        public TParameters Parameters
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
            var count = info.Container.ListData.MarkerOffset + info.Container.ListData.Padding;
            if (info.Indent >= count)
            {
                info.AdvanceOffset(count, true);
                return true;
            }
            if (info.IsBlank && info.Container.FirstChild != null)
            {
                // if container.FirstChild is null, then the opening line
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
        protected virtual bool CanOpen(BlockParserInfo info)
        {
            return (!info.IsIndented || info.Container.Tag == ParentTag);
        }

        /// <summary>
        /// Opens a list item.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="canOpen">Opener checker delegate.</param>
        /// <param name="parseMarker">Marker parser delegate.</param>
        /// <param name="isListsMatch">List data matcher delegate.</param>
        /// <param name="setListData">List data mutator delegate.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected bool DoOpen(BlockParserInfo info, Func<BlockParserInfo, bool> canOpen, ParseMarkerDelegate parseMarker,
            Func<BlockParserInfo, TData, bool> isListsMatch, Action<BlockParserInfo, TData> setListData)
        {
            int markerLength;
            ListData data;
            TData listData;
            if (!canOpen(info) || 0 == (markerLength = parseMarker(info, out data, out listData)))
                return false;

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
            if (info.Container.Tag != ParentTag || !isListsMatch(info, listData))
            {
                info.Container = CreateChildBlock(info, ParentTag, info.FirstNonspace);
                info.Container.ListData = data;
                setListData(info, listData);
            }

            // add the list item
            info.Container = CreateChildBlock(info, Tag, info.FirstNonspace);
            info.Container.ListData = data;
            setListData(info, listData);

            return true;
        }

        /// <summary>
        /// Attempts to parse a list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected abstract int ParseMarker(BlockParserInfo info, out ListData data, out TData listData);

        /// <summary>
        /// Determines whether a list item belongs to a list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected abstract bool IsListsMatch(BlockParserInfo info, TData listData);

        /// <summary>
        /// Updates a container with specific list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Specific list data.</param>
        protected abstract void SetListData(BlockParserInfo info, TData listData);

        /// <summary>
        /// Scans the line past the delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="offset">Current offset.</param>
        /// <param name="curChar">Current character.</param>
        /// <param name="start">First item string.</param>
        /// <param name="bulletChar">Bullet character.</param>
        /// <param name="delimiter">Delimiter character.</param>
        /// <param name="startFactory">Calculates an integer start value.</param>
        /// <param name="listDataFactory">Creates and initializes a list data object.</param>
        /// <param name="data">Common list data object.</param>
        /// <param name="listData">Specific list data object.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected int CompleteScan(BlockParserInfo info, int offset, string start, char curChar, char bulletChar, char delimiter,
            Func<string, int> startFactory, Func<char, string, int, TData> listDataFactory, out ListData data, out TData listData)
        {
            data = null;
            listData = null;

            var delimIndex = System.Array.IndexOf(DelimiterCharacters, curChar);
            if (delimIndex < 0)
                return 0;

            var line = info.Line;
            var length = line.Length;
            var delimOffset = ++offset;

            while (offset < length && line[offset] == ' ')
                offset++;

            if (offset - delimOffset < MinDelimiterSpaces[delimIndex] && offset < length && line[offset] != '\n')
                return 0;

            var markerLength = delimOffset - info.FirstNonspace;
            if (markerLength == 0)
                return 0;

            var intStart = startFactory(start);

            data = new ListData
            {
#pragma warning disable 0618
                ListType = ListType,
                Start = intStart,
                BulletChar = bulletChar,
#pragma warning restore 0618
            };

#pragma warning disable 0618
            if (delimiter == ')')
                data.Delimiter = ListDelimiter.Parenthesis;
#pragma warning restore 0618

            listData = listDataFactory(curChar, start, intStart);

            return markerLength;
        }

        private void SetDelimiters(ListItemDelimiterParameters[] delimiters)
        {
            var length = delimiters.Length;
            DelimiterCharacters = new char[length];
            MinDelimiterSpaces = new int[length];
            for (var i = 0; i < length; i++)
            {
                DelimiterCharacters[i] = delimiters[i].Character;
                MinDelimiterSpaces[i] = delimiters[i].MinSpaces;
            }
        }

        private char[] DelimiterCharacters
        {
            get;
            set;
        }

        private int[] MinDelimiterSpaces
        {
            get;
            set;
        }
    }
}
