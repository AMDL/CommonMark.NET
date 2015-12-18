using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list item element parser for markers that are NOT shared with horizontal rules.
    /// </summary>
    public sealed class NonRuleBulletListItemParser : BulletListItemParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly BulletListParameters DefaultParameters = new BulletListParameters(new[]
            {
                new ListItemDelimiterParameters('+'),
                new ListItemDelimiterParameters('•'),
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="NonRuleBulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parentTag">Parent element tag.</param>
        /// <param name="parameters">Bullet list parameters.</param>
        public NonRuleBulletListItemParser(CommonMarkSettings settings, BlockTag parentTag, BulletListParameters parameters)
            : base(settings, parentTag, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonRuleBulletListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public NonRuleBulletListItemParser(CommonMarkSettings settings)
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
    }
}
