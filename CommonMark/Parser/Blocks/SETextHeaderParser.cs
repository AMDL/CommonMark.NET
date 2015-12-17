using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.SETextHeader"/> element parser.
    /// </summary>
    public class SETextHeaderParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public SETextHeaderParser(CommonMarkSettings settings)
            : this(settings, new[] { '=', '-' })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SETextHeaderParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="headerLevels">Mapping from (level-1) to character.</param>
        protected SETextHeaderParser(CommonMarkSettings settings, char[] headerLevels)
            : base(settings, headerLevels)
        {
            // we don't count setext headers for purposes of tight/loose lists or breaking out of lists.
            IsAlwaysDiscardBlanks = true;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            // a header can never contain more than one line
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
            }
            return false;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            int headerLevel;
            if (!info.IsIndented && info.Container.Tag == BlockTag.Paragraph && 0 != (headerLevel = ScanLine(info))
                && ContainsSingleLine(info.Container.StringContent))
            {
                info.Container.Tag = BlockTag.SETextHeader;
                info.Container.HeaderLevel = headerLevel;
                info.AdvanceOffset(info.Line.Length - 1 - info.Offset, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Close(BlockParserInfo info)
        {
            return true;
        }

        /// <summary>
        /// Processes the inline contents of a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="inlineStack">Inline stack.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Process(Block container, Subject subject, ref Stack<Inline> inlineStack)
        {
            return ProcessInlines(container, subject, ref inlineStack, Settings.InlineParserParameters);
        }

        /// <summary>
        /// Matches sexext header line.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>Header level, or 0 for no match.</returns>
        /// <remarks>Original: int scan_setext_header_line(string s, int pos, int sourceLength)</remarks>
        private int ScanLine(BlockParserInfo info)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            /*!re2c
              [=]+ [ ]* [\n] { return 1; }
              [-]+ [ ]* [\n] { return 2; }
              .? { return 0; }
            */

            if (pos >= sourceLength)
                return 0;

            var c1 = s[pos];

            var chars = Characters;
            var matched = System.Array.IndexOf(chars, c1) + 1;
            if (matched == 0)
                return 0;

            var fin = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == c1 && !fin)
                    continue;

                fin = true;
                if (c == ' ')
                    continue;

                if (c == '\n')
                    break;

                return 0;
            }

            return matched;
        }
    }
}
