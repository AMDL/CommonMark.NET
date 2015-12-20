using CommonMark.Syntax;
using System;
using System.Collections.Generic;

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
    /// List item parameters.
    /// </summary>
    public interface IListItemParameters
    {
        /// <summary>
        /// Gets the list element tag.
        /// </summary>
        BlockTag ParentTag { get; }

        /// <summary>
        /// Gets the list type.
        /// </summary>
        [Obsolete("This API has been superceded by " + nameof(BlockTag.BulletList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        ListType ListType { get; }
    }

    /// <summary>
    /// Base list item parameters class.
    /// </summary>
    /// <typeparam name="TDelimiterParameters">Type of delimiter parameters.</typeparam>
    public abstract class ListItemParameters<TDelimiterParameters> : IListItemParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParameters{TDelimiterParameters}"/> class.
        /// </summary>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        protected ListItemParameters(BlockTag parentTag, ListType listType, TDelimiterParameters[] delimiters)
        {
            this.ParentTag = parentTag;
            this.ListType = listType;
            this.Delimiters = delimiters;
        }
#pragma warning restore 0618

        /// <summary>
        /// Gets or sets the list element tag.
        /// </summary>
        public BlockTag ParentTag { get; set; }

        /// <summary>
        /// Gets or sets the list type.
        /// </summary>
        [Obsolete("This API has been superceded by " + nameof(BlockTag.BulletList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        public ListType ListType { get; set; }

        /// <summary>
        /// Gets or sets the delimiter parameters.
        /// </summary>
        public TDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// List parameters.
    /// </summary>
    public sealed class ListParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListParameters"/> class.
        /// </summary>
        /// <param name="items">List item parameters.</param>
        public ListParameters(params IListItemParameters[] items)
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the list item parameters.
        /// </summary>
        public IListItemParameters[] Items { get; set; }
    }

    /// <summary>
    /// Base <see cref="BlockTag.ListItem"/> element parser class.
    /// </summary>
    public sealed class ListItemParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly BulletListItemParameters DefaultBulletListItemParameters = new BulletListItemParameters(
            BlockTag.BulletList,
#pragma warning disable 0618
            ListType.Bullet,
#pragma warning restore 0618
            new BulletListItemDelimiterParameters('*', isHorizontalRuleCharacter: true),
            new BulletListItemDelimiterParameters('-', isHorizontalRuleCharacter: true),
            new BulletListItemDelimiterParameters('+', isHorizontalRuleCharacter: false),
            new BulletListItemDelimiterParameters('•', isHorizontalRuleCharacter: false));

        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly OrderedListItemParameters DefaultOrderedListItemParameters = new OrderedListItemParameters(
            BlockTag.OrderedList,
#pragma warning disable 0618
            ListType.Ordered,
#pragma warning restore 0618
            '0', '9', 9,
            new ListItemDelimiterParameters('.'),
            new ListItemDelimiterParameters(')'));

        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly ListParameters DefaultParameters = new ListParameters(
            DefaultBulletListItemParameters,
            DefaultOrderedListItemParameters);

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parameters">List parameters.</param>
#pragma warning disable 0618
        public ListItemParser(CommonMarkSettings settings, BlockTag tag = BlockTag.ListItem, ListParameters parameters = null)
#pragma warning restore 0618
            : base(settings, tag)
        {
            Parameters = parameters ?? DefaultParameters;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                foreach (var item in Parameters.Items)
                {
                    var bulletListItemParameters = item as BulletListItemParameters;
                    if (bulletListItemParameters != null)
                    {
                        foreach (var delimiter in bulletListItemParameters.Delimiters)
                        {
                            yield return new BulletListItemHandler(Settings, Tag, bulletListItemParameters, delimiter);
                        }
                    }
                    var orderedListItemParameters = item as OrderedListItemParameters;
                    if (orderedListItemParameters != null)
                    {
                        for (var i = 0; i <= orderedListItemParameters.MarkerLast - orderedListItemParameters.MarkerFirst; i++)
                        {
                            yield return new OrderedListItemHandler(Settings, Tag, (char)(i + orderedListItemParameters.MarkerFirst), orderedListItemParameters);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list parameters.
        /// </summary>
        public ListParameters Parameters
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
        /// <param name="tag">List item element tag.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        protected ListItemHandler(CommonMarkSettings settings, BlockTag tag, char character, IListItemParameters parameters, params ListItemDelimiterParameters[] delimiters)
            : base(settings, tag, character)
        {
            ParentTag = parameters.ParentTag;
            ListType = parameters.ListType;
            SetDelimiters(delimiters);
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
        [Obsolete("This API has been superceded by " + nameof(BlockTag.BulletList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        public ListType ListType
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
            info.Container = BlockParser.CreateChildBlock(info, Tag, info.FirstNonspace, Settings);
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
        /// <param name="startStr">First item string.</param>
        /// <param name="bulletChar">Bullet character.</param>
        /// <param name="delimiter">Delimiter character.</param>
        /// <param name="startFactory">Calculates an integer start value.</param>
        /// <param name="listDataFactory">Creates and initializes a list data object.</param>
        /// <param name="data">Common list data object.</param>
        /// <param name="listData">Specific list data object.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        protected int CompleteScan(BlockParserInfo info, int offset, string startStr, char curChar, char bulletChar, char delimiter,
            Func<string, int> startFactory, Func<char, int, TData> listDataFactory, out ListData data, out TData listData)
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

            var start = startFactory(startStr);

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
        /// Calculates an integer start value.
        /// </summary>
        /// <param name="startStr">Start string.</param>
        /// <returns>Integer start value, or 1 if unsuccessful.</returns>
        protected abstract int GetStart(string startStr);

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
