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
        /// Gets the value indicating whether the delimiter character doubles as a thematic break character.
        /// </summary>
        public bool IsThematicBreakCharacter { get; }

        /// <summary>
        /// Gets the list style.
        /// </summary>
        public string ListStyle { get; }
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
                : base(parameters, new[] { delimiter.Character }, delimiter, false)
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
            new UnorderedListItemDelimiterParameters('+'));

        /// <summary>
        /// Creates an unordered list item delimiter handler.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <returns>Delegate delimiter handler.</returns>
        public static IBlockDelimiterHandler Create(CommonMarkSettings settings, UnorderedListItemParameters parameters)
        {
            var characters = GetCharacters(parameters);
            var handlers = GetHandlers(settings, parameters);
            return DelegateBlockDelimiterHandler.Merge(characters, handlers);
        }

        private static char[] GetCharacters(UnorderedListItemParameters parameters)
        {
            var length = parameters.Delimiters.Length;
            var characters = new char[length];
            for (var i = 0; i < length; i++)
                characters[i] = parameters.Delimiters[i].Character;
            return characters;
        }

        private static IBlockDelimiterHandler[] GetHandlers(CommonMarkSettings settings, UnorderedListItemParameters parameters)
        {
            var length = parameters.Delimiters.Length;
            var handlers = new IBlockDelimiterHandler[length];
            for (var i = 0; i < length; i++)
                handlers[i] = new UnorderedListItemHandler(settings, parameters, parameters.Delimiters[i]);
            return handlers;
        }

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
            return DoHandle(info, CanOpen, ParseMarker, null, MatchList, SetList);
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
        /// Attempts to parse an unordered list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="adjustStart">Start value adjuster delegate.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="list">Unordered list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
#pragma warning disable 0618
        protected override int ParseMarker(BlockParserInfo info, AdjustStartDelegate adjustStart, out ListData data, out UnorderedListData list)
#pragma warning restore 0618
        {
            data = null;
            list = null;

            var curChar = info.CurrentCharacter;
            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            if (offset == length - 1)
                return 0;

            return CompleteScan(info, offset, 1, curChar, curChar, '\0', GetList, out data, out list);
        }

        /// <summary>
        /// Matches a list item to an unordered list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="list">Unordered list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="list"/>.</returns>
        protected override bool MatchList(BlockParserInfo info, UnorderedListData list)
        {
            var containerList = info.Container.UnorderedList;
            return containerList != null
                && containerList.BulletCharacter == list.BulletCharacter
                && ((containerList.Style == null && list.Style == null) || (containerList.Style != null && containerList.Style.Equals(list.Style)));
        }

        /// <summary>
        /// Updates a container with unordered list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="list">Unordered list data.</param>
        protected override void SetList(BlockParserInfo info, UnorderedListData list)
        {
            info.Container.UnorderedList = list;
        }

        /// <summary>
        /// Creates and initializes unordered list data.
        /// </summary>
        /// <param name="curChar">Current character.</param>
        /// <param name="start">Start value.</param>
        /// <returns></returns>
        protected override UnorderedListData GetList(char curChar, int start)
        {
            return new UnorderedListData
            {
                BulletCharacter = curChar,
                Style = ListStyle,
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
