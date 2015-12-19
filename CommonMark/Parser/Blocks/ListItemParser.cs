using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// List item delimiter parameters.
    /// </summary>
    public class ListItemDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemDelimiterParameters"/> class.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="minSpaces">Minimum space count.</param>
        public ListItemDelimiterParameters(char character, int minSpaces = 1)
        {
            Character = character;
            MinSpaces = minSpaces;
        }

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of space characters between the delimiter and the item content.
        /// </summary>
        public int MinSpaces { get; set; }
    }

    /// <summary>
    /// Base <see cref="BlockTag.ListItem"/> element parser class.
    /// </summary>
    /// <typeparam name="TParameters">Type of parameters.</typeparam>
    public abstract class ListItemParser<TParameters> : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParser{TParameters}"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="listType">List type.</param>
        /// <param name="parameters">List parameters.</param>
#pragma warning disable 0618
        public ListItemParser(CommonMarkSettings settings, BlockTag tag, BlockTag parentTag, ListType listType, TParameters parameters)
#pragma warning restore 0618
            : base(settings, tag)
        {
            ParentTag = parentTag;
            Parameters = parameters;
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
    }

    /// <summary>
    /// Base list item handler class.
    /// </summary>
    /// <typeparam name="TData">Type of specific list data.</typeparam>
    public abstract class ListItemHandler<TData> : BlockDelimiterHandler
        where TData : class
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
        /// Initializes a new instance of the <see cref="ListItemHandler{TData}"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="blockTag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="character">Handled character.</param>
        /// <param name="delimiters">List item delimiters.</param>
#pragma warning disable 0618
        protected ListItemHandler(CommonMarkSettings settings, BlockTag blockTag, BlockTag parentTag, ListType listType, char character, ListItemDelimiterParameters[] delimiters)
#pragma warning restore 0618
            : base(settings, character)
        {
            ParentTag = parentTag;
            ListType = listType;
            SetDelimiters(delimiters);
        }

        /// <summary>
        /// Gets the element tag of the parent list.
        /// </summary>
        public BlockTag ParentTag
        {
            get;
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
        /// Determines whether a list item can be opened.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the current line may contain a handled list item element.</returns>
        protected virtual bool CanOpen(BlockParserInfo info)
        {
            return (!info.IsIndented || info.Container.Tag == ParentTag);
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="canOpen">Opener checker delegate.</param>
        /// <param name="parseMarker">Marker parser delegate.</param>
        /// <param name="isListsMatch">List data matcher delegate.</param>
        /// <param name="setListData">List data mutator delegate.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected bool DoHandle(BlockParserInfo info, Func<BlockParserInfo, bool> canOpen, ParseMarkerDelegate parseMarker,
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
                info.Container = BlockParser.CreateChildBlock(info, ParentTag, info.FirstNonspace, Settings);
                info.Container.ListData = data;
                setListData(info, listData);
            }

            // add the list item
            info.Container = BlockParser.CreateChildBlock(info, BlockTag.ListItem, info.FirstNonspace, Settings);
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
