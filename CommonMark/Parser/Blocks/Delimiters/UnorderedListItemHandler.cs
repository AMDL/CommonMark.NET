using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Unordered list item delimiter parameters.
    /// </summary>
    public sealed class UnorderedListItemDelimiterParameters : ListItemDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedListItemDelimiterParameters"/> class.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="isThematicBreakCharacter"><c>true</c> if the delimiter character doubles as a thematic break character.</param>
        /// <param name="listStyle">List style.</param>
        /// <param name="minSpaceCount">Minimum space count.</param>
        public UnorderedListItemDelimiterParameters(char character, bool isThematicBreakCharacter = false, string listStyle = null, int minSpaceCount = 1)
            : base(character, minSpaceCount)
        {
            this.IsThematicBreakCharacter = isThematicBreakCharacter;
            this.ListStyle = listStyle;
        }

        /// <summary>
        /// Gets or sets the value indicating whether the delimiter character doubles as a thematic break character.
        /// </summary>
        public bool IsThematicBreakCharacter { get; set; }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        public string ListStyle { get; set; }
    }

    /// <summary>
    /// Unordered list item delimiter handler.
    /// </summary>
    public sealed class UnorderedListItemHandler : ListItemHandler<UnorderedListData, UnorderedListItemParameters, UnorderedListItemHandler.Parameters>
    {
        /// <summary>
        /// Handler parameters.
        /// </summary>
        public sealed class Parameters : ListItemHandlerParameters<UnorderedListItemParameters>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Parameters"/> class.
            /// </summary>
            /// <param name="parameters">List item parameters.</param>
            /// <param name="delimiter">Delimiter parameters.</param>
            public Parameters(UnorderedListItemParameters parameters, UnorderedListItemDelimiterParameters delimiter)
                : base(parameters, new[] { delimiter.Character }, new[] { delimiter }, false)
            {
            }
        }

        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly UnorderedListItemParameters DefaultParameters = new UnorderedListItemParameters(
            BlockTag.ListItem,
            BlockTag.UnorderedList,
#pragma warning disable 0618
            ListType.Bullet,
#pragma warning restore 0618
            new UnorderedListItemDelimiterParameters('*', isThematicBreakCharacter: true),
            new UnorderedListItemDelimiterParameters('-', isThematicBreakCharacter: true),
            new UnorderedListItemDelimiterParameters('+'),
            new UnorderedListItemDelimiterParameters('•'));

        /// <summary>
        /// Initializes a new instance of the <see cref="UnorderedListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public UnorderedListItemHandler(CommonMarkSettings settings, UnorderedListItemParameters parameters, UnorderedListItemDelimiterParameters delimiter)
            : base(settings, GetHandlerParameters(parameters, delimiter))
        {
            IsThematicBreakCharacter = delimiter.IsThematicBreakCharacter;
            ListStyle = delimiter.ListStyle;
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, null, MatchList, SetListData);
        }

        /// <summary>
        /// Determines whether a list item can be opened.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the current line may contain a handled list item element.</returns>
        protected override bool CanOpen(BlockParserInfo info)
        {
            return base.CanOpen(info) && !(IsThematicBreakCharacter && ScanThematicBreak(info, info.CurrentCharacter));
        }

        /// <summary>
        /// Matches a list item to an unordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Unordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected override bool MatchList(BlockParserInfo info, UnorderedListData listData)
        {
            return listData.Equals(info.Container.UnorderedListData);
        }

        /// <summary>
        /// Updates a container with unordered list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Unordered list data.</param>
        protected override void SetListData(BlockParserInfo info, UnorderedListData listData)
        {
            info.Container.UnorderedListData = listData;
        }

        /// <summary>
        /// Attempts to parse an unordered list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="adjustStart">Start value adjuster delegate.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Unordered list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected override int ParseMarker(BlockParserInfo info, AdjustStartDelegate adjustStart, out ListData data, out UnorderedListData listData)
        {
            data = null;
            listData = null;

            var curChar = info.CurrentCharacter;
            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            if (offset == length - 1)
                return 0;

            return CompleteScan(info, offset, 1, curChar, curChar, '\0', GetListData, out data, out listData);
        }

        /// <summary>
        /// Creates and initializes unordered list data.
        /// </summary>
        /// <param name="curChar">Current character.</param>
        /// <param name="start">Start value.</param>
        /// <returns></returns>
        protected override UnorderedListData GetListData(char curChar, int start)
        {
            return new UnorderedListData
            {
                BulletCharacter = curChar,
                ListStyle = ListStyle,
            };
        }

        private bool IsThematicBreakCharacter
        {
            get;
        }

        private string ListStyle
        {
            get;
        }

        private static Parameters GetHandlerParameters(UnorderedListItemParameters parameters, UnorderedListItemDelimiterParameters delimiter)
        {
            return new Parameters(parameters, delimiter);
        }
    }
}
