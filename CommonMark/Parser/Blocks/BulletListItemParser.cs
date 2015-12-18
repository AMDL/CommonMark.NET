using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list parameters.
    /// </summary>
    public sealed class BulletListParameters : ListParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListParameters"/> class.
        /// </summary>
        /// <param name="delimiters">Delimiter parameters.</param>
        public BulletListParameters(ListItemDelimiterParameters[] delimiters)
            : base(delimiters)
        {
        }
    }

    /// <summary>
    /// Bullet list item element parser.
    /// </summary>
    public abstract class BulletListItemParser : ListItemParser<BulletListData, BulletListParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="parameters">Bullet list parameters.</param>
        public BulletListItemParser(CommonMarkSettings settings, BlockTag parentTag, BulletListParameters parameters)
#pragma warning disable 0618
            : base(settings, BlockTag.ListItem, parentTag, ListType.Bullet, GetCharacters(parameters), parameters)
#pragma warning restore 0618
        {
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

        private static char[] GetCharacters(BulletListParameters parameters)
        {
            var length = parameters.Delimiters.Length;
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = parameters.Delimiters[i].Character;
            return chars;
        }
    }
}
