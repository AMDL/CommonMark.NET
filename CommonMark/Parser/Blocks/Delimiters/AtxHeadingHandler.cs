using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// ATX heading delimiter handler.
    /// </summary>
    public sealed class AtxHeadingHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtxHeadingHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="opener">Opener character.</param>
        public AtxHeadingHandler(CommonMarkSettings settings, BlockTag tag, char opener)
            : base(settings, tag, opener)
        {
            Opener = opener;
        }

        /// <summary>
        /// Handles a block delimiter.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            int offset;
            int headingLevel;
            if (!info.IsIndented && 0 != (offset = ScanStart(info, out headingLevel)))
            {
                info.AdvanceOffset(info.FirstNonspace + offset - info.Offset, false);
                info.Container = AppendChildBlock(info, Tag, info.FirstNonspace);
                info.Container.Heading = new HeadingData(headingLevel);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Matches an ATX heading start.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="headingLevel">Heading level.</param>
        /// <returns>Offset, or 0 for no match.</returns>
        /// <remarks>Original: int scan_atx_heading_start(string s, int pos, int sourceLength, out int headingLevel)</remarks>
        private int ScanStart(BlockParserInfo info, out int headingLevel)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            /*!re2c
              [#]{1,6} ([ ]+|[\n])  { return (p - start); }
              .? { return 0; }
            */

            headingLevel = 1;
            if (pos + 1 >= sourceLength)
                return 0;

            var spaceExists = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (c == Opener)
                {
                    if (headingLevel == 6)
                        return 0;

                    if (spaceExists)
                        return i - pos;
                    else
                        headingLevel++;
                }
                else if (c == ' ')
                {
                    spaceExists = true;
                }
                else if (c == '\n')
                {
                    return i - pos + 1;
                }
                else
                {
                    return spaceExists ? i - pos : 0;
                }
            }

            if (spaceExists)
                return sourceLength - pos;

            return 0;
        }

        private char Opener
        {
            get;
        }
    }
}
