using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// List item delimiter parameters.
    /// </summary>
    public class ListItemDelimiterParameters
    {
        /// <summary>
        /// The default delimiter parameters for ordered lists.
        /// </summary>
        public static readonly ListItemDelimiterParameters[] Default = new[]
        {
            new ListItemDelimiterParameters('.', 1),
            new ListItemDelimiterParameters(')', 1),
        };

        /// <summary>
        /// The default delimiter parameters for ordered lists with uppercase ASCII letter markers.
        /// </summary>
        public static readonly ListItemDelimiterParameters[] DefaultUpper = new[]
        {
            new ListItemDelimiterParameters('.', 2),
            new ListItemDelimiterParameters(')', 1),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemDelimiterParameters"/> class.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="minSpaceCount">Minimum space count.</param>
        public ListItemDelimiterParameters(char character, int minSpaceCount = 1)
        {
            Character = character;
            MinSpaceCount = minSpaceCount;
        }

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of space characters between the delimiter and the item content.
        /// </summary>
        public int MinSpaceCount { get; set; }
    }

    /// <summary>
    /// Base list item delimiter handler parameters class.
    /// </summary>
    /// <typeparam name="TParameters">Type of list item parameters.</typeparam>
    public abstract class ListItemHandlerParameters<TParameters>
        where TParameters : IListItemParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemHandlerParameters{TParameters}"/> class.
        /// </summary>
        /// <param name="parameters">List item parameters.</param>
        /// <param name="characters">Handled characters.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
        /// <param name="isRequireContent"><c>true</c> if items on this list require content.</param>
        protected ListItemHandlerParameters(TParameters parameters, char[] characters, ListItemDelimiterParameters[] delimiters, bool isRequireContent)
        {
            Parameters = parameters;
            Characters = characters;
            Delimiters = delimiters;
            IsRequireContent = isRequireContent;
        }

        /// <summary>
        /// Gets the list item parameters.
        /// </summary>
        public TParameters Parameters { get; }

        /// <summary>
        /// Gets the handled characters.
        /// </summary>
        public char[] Characters { get; }

        /// <summary>
        /// Gets the delimiter parameters.
        /// </summary>
        public ListItemDelimiterParameters[] Delimiters { get; }

        /// <summary>
        /// Gets or sets the value indicating whether items on this list require content.
        /// </summary>
        public bool IsRequireContent { get; }
    }

    /// <summary>
    /// Base list item delimiter handler class.
    /// </summary>
    /// <typeparam name="TData">Type of specific list data.</typeparam>
    /// <typeparam name="TParameters">Type of list item parameters.</typeparam>
    /// <typeparam name="THandlerParameters">Type of handler parameters.</typeparam>
    public abstract class ListItemHandler<TData, TParameters, THandlerParameters> : BlockDelimiterHandler
        where TData : class
        where TParameters : IListItemParameters
        where THandlerParameters : ListItemHandlerParameters<TParameters>
    {
        /// <summary>
        /// Start value adjuster delegate.
        /// </summary>
        /// <param name="start">Current start value.</param>
        /// <param name="value">Current character value.</param>
        /// <param name="curChar">Current character.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected delegate bool AdjustStartDelegate(ref int start, ref int value, char curChar);

        /// <summary>
        /// List marker parser delegate.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="adjustStart">Start value adjuster delegate.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected delegate int ParseMarkerDelegate(BlockParserInfo info, AdjustStartDelegate adjustStart, out ListData data, out TData listData);

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemHandler{TData,TParameters,THandlerParameters}"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="handlerParameters">Handler parameters.</param>
#pragma warning disable 0618
        protected ListItemHandler(CommonMarkSettings settings, THandlerParameters handlerParameters)
            : base(settings, handlerParameters.Parameters.Tag, handlerParameters.Characters)
        {
            HandlerParameters = handlerParameters;
            ParentTag = handlerParameters.Parameters.ParentTag;
            ListType = handlerParameters.Parameters.ListType;
            IsRequireContent = handlerParameters.IsRequireContent;
            SetDelimiters(handlerParameters.Delimiters);
        }
#pragma warning restore 0618

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
        [Obsolete("This API has been superceded by " + nameof(BlockTag.UnorderedList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        public ListType ListType
        {
            get;
        }

        /// <summary>
        /// Gets the handler parameters.
        /// </summary>
        protected THandlerParameters HandlerParameters
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
        /// <param name="adjustStart">Start value adjuster delegate.</param>
        /// <param name="matchList">List data matcher delegate.</param>
        /// <param name="setListData">List data mutator delegate.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected bool DoHandle(BlockParserInfo info, Func<BlockParserInfo, bool> canOpen, ParseMarkerDelegate parseMarker,
            AdjustStartDelegate adjustStart, Func<BlockParserInfo, TData, bool> matchList, Action<BlockParserInfo, TData> setListData)
        {
            int markerLength;
            ListData data;
            TData listData;
            if (!canOpen(info) || 0 == (markerLength = parseMarker(info, adjustStart, out data, out listData)))
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
            if (info.Container.Tag != ParentTag || !matchList(info, listData))
            {
                info.Container = AppendChildBlock(info, ParentTag, info.FirstNonspace);
                info.Container.ListData = data;
                setListData(info, listData);
            }

            // add the list item
            info.Container = AppendChildBlock(info, Tag, info.FirstNonspace);
            info.Container.ListData = data;
            setListData(info, listData);

            return true;
        }

        /// <summary>
        /// Attempts to parse a list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="adjustStart">Start value adjuster delegate.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected abstract int ParseMarker(BlockParserInfo info, AdjustStartDelegate adjustStart, out ListData data, out TData listData);

        /// <summary>
        /// Matches a list item to a list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected abstract bool MatchList(BlockParserInfo info, TData listData);

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
        /// <param name="start">Start value.</param>
        /// <param name="bulletChar">Bullet character.</param>
        /// <param name="delimiter">Delimiter character.</param>
        /// <param name="listDataFactory">Creates and initializes a list data object.</param>
        /// <param name="data">Common list data object.</param>
        /// <param name="listData">Specific list data object.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected int CompleteScan(BlockParserInfo info, int offset, int start, char curChar, char bulletChar, char delimiter,
            Func<char, int, TData> listDataFactory, out ListData data, out TData listData)
        {
            data = null;
            listData = null;

            var delimIndex = System.Array.IndexOf(DelimiterCharacters, curChar);
            if (delimIndex < 0)
                return 0;

            var line = info.Line;
            var length = line.Length;
            var delimOffset = ++offset;

            if (offset == length - 1 && IsRequireContent)
                return 0;

            while (offset < length && line[offset] == ' ')
                offset++;

            if (offset == length - 1 && IsRequireContent)
                return 0;

            if (offset - delimOffset < MinSpaceCounts[delimIndex] && offset < length && line[offset] != '\n')
                return 0;

            var markerLength = delimOffset - info.FirstNonspace;
            if (markerLength == 0)
                return 0;

            data = new ListData
            {
#pragma warning disable 0618
                ListType = ListType,
                Start = start,
                BulletChar = bulletChar,
#pragma warning restore 0618
            };

#pragma warning disable 0618
            if (delimiter == ')')
                data.Delimiter = ListDelimiter.Parenthesis;
#pragma warning restore 0618

            listData = listDataFactory(curChar, start);

            return markerLength;
        }

        /// <summary>
        /// Creates and initializes specific list data.
        /// </summary>
        /// <param name="curChar">Current character.</param>
        /// <param name="start">Start value.</param>
        /// <returns></returns>
        protected abstract TData GetListData(char curChar, int start);

        private void SetDelimiters(ListItemDelimiterParameters[] delimiters)
        {
            var length = delimiters.Length;
            DelimiterCharacters = new char[length];
            MinSpaceCounts = new int[length];
            for (var i = 0; i < length; i++)
            {
                DelimiterCharacters[i] = delimiters[i].Character;
                MinSpaceCounts[i] = delimiters[i].MinSpaceCount;
            }
        }

        private char[] DelimiterCharacters
        {
            get;
            set;
        }

        private int[] MinSpaceCounts
        {
            get;
            set;
        }

        private bool IsRequireContent
        {
            get;
        }
    }
}
