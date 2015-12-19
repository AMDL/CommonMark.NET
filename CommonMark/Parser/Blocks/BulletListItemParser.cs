using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list item delimiter parameters.
    /// </summary>
    public sealed class BulletListItemDelimiterParameters : ListItemDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemDelimiterParameters"/> class.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="isHorizontalRuleCharacter"><c>true</c> if the delimiter character doubles as a horizontal rule character.</param>
        /// <param name="minSpaces">Minimum space count.</param>
        public BulletListItemDelimiterParameters(char character, bool isHorizontalRuleCharacter, int minSpaces = 1)
            : base(character, minSpaces)
        {
            this.IsHorizontalRuleCharacter = isHorizontalRuleCharacter;
        }

        /// <summary>
        /// Gets or sets the value indicating whether the delimiter character doubles as a horizontal rule character.
        /// </summary>
        public bool IsHorizontalRuleCharacter { get; set; }
    }

    /// <summary>
    /// Bullet list item parameters.
    /// </summary>
    public sealed class BulletListItemParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemParameters"/> class.
        /// </summary>
        /// <param name="delimiters">Delimiter parameters.</param>
        public BulletListItemParameters(BulletListItemDelimiterParameters[] delimiters)
        {
            this.Delimiters = delimiters;
        }

        /// <summary>
        /// Gets or sets the delimiter parameters.
        /// </summary>
        public BulletListItemDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// Bullet list item element parser.
    /// </summary>
    public sealed class BulletListItemParser : ListItemParser<BulletListItemParameters>
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly BulletListItemParameters DefaultParameters = new BulletListItemParameters(new[]
            {
                new BulletListItemDelimiterParameters('*', isHorizontalRuleCharacter: true),
                new BulletListItemDelimiterParameters('-', isHorizontalRuleCharacter: true),
                new BulletListItemDelimiterParameters('+', isHorizontalRuleCharacter: false),
                new BulletListItemDelimiterParameters('•', isHorizontalRuleCharacter: false),
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="parameters">Bullet list parameters.</param>
        public BulletListItemParser(CommonMarkSettings settings, BlockTag parentTag = BlockTag.BulletList, BulletListItemParameters parameters = null)
#pragma warning disable 0618
            : base(settings, BlockTag.ListItem, parentTag, ListType.Bullet, parameters ?? DefaultParameters)
#pragma warning restore 0618
        {
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                foreach (var delimiter in Parameters.Delimiters)
                {
                    yield return new BulletListItemHandler(Settings, ParentTag, delimiter);
                }
            }
        }
    }

    /// <summary>
    /// Bullet list item element parser.
    /// </summary>
    public sealed class BulletListItemHandler : ListItemHandler<BulletListData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public BulletListItemHandler(CommonMarkSettings settings, BlockTag parentTag, BulletListItemDelimiterParameters delimiter)
#pragma warning disable 0618
            : base(settings, BlockTag.ListItem, parentTag, ListType.Bullet, delimiter.Character, new[] { delimiter })
#pragma warning restore 0618
        {
            Delimiter = delimiter;
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, IsListsMatch, SetListData);
        }

        /// <summary>
        /// Determines whether a list item can be opened.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the current line may contain a handled list item element.</returns>
        protected override bool CanOpen(BlockParserInfo info)
        {
            return base.CanOpen(info) && !(Delimiter.IsHorizontalRuleCharacter && ScanHorizontalRule(info, info.CurrentCharacter));
        }

        /// <summary>
        /// Determines whether a list item belongs to a bullet list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="bulletListData">Bullet list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="bulletListData"/>.</returns>
        protected override bool IsListsMatch(BlockParserInfo info, BulletListData bulletListData)
        {
            return bulletListData.Equals(info.Container.BulletListData);
        }

        /// <summary>
        /// Updates a container with bullet list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="bulletListData">Bullet list data.</param>
        protected override void SetListData(BlockParserInfo info, BulletListData bulletListData)
        {
            info.Container.BulletListData = bulletListData;
        }

        /// <summary>
        /// Attempts to parse a bullet list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="bulletListData">Bullet list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected override int ParseMarker(BlockParserInfo info, out ListData data, out BulletListData bulletListData)
        {
            data = null;
            bulletListData = null;

            var curChar = info.CurrentCharacter;
            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            if (offset == length - 1)
                return 0;

            return CompleteScan(info, offset, null, curChar, curChar, '\0',
                GetIntStart, GetBulletListData, out data, out bulletListData);
        }

        private static int GetIntStart(string start)
        {
            return 1;
        }

        private static BulletListData GetBulletListData(char curChar, string start, int intStart)
        {
            return new BulletListData
            {
                BulletCharacter = curChar,
            };
        }

        private BulletListItemDelimiterParameters Delimiter
        {
            get;
        }
    }
}
