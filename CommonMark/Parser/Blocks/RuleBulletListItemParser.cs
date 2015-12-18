using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list item element parser for markers that are shared with horizontal rules.
    /// </summary>
    public sealed class RuleBulletListItemParser : BulletListItemParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly BulletListParameters DefaultParameters = new BulletListParameters(new[]
            {
                new ListItemDelimiterParameters('*'),
                new ListItemDelimiterParameters('-'),
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="parameters">Bullet list parameters.</param>
        public RuleBulletListItemParser(CommonMarkSettings settings, BlockTag parentTag, BulletListParameters parameters)
            : base(settings, parentTag, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public RuleBulletListItemParser(CommonMarkSettings settings)
            : this(settings, BlockTag.BulletList, DefaultParameters)
        {
        }

        /// <summary>
        /// Opens a list item.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            return DoOpen(info, CanOpen, ParseMarker, IsListsMatch, SetListData);
        }

        /// <summary>
        /// Determines whether a list item can be opened.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the current line may contain a handled list item element.</returns>
        protected override bool CanOpen(BlockParserInfo info)
        {
            return base.CanOpen(info) && !ScanHorizontalRule(info, info.CurrentCharacter);
        }
    }
}
