using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.AtxHeader"/> element parser.
    /// </summary>
    public class AtxHeaderParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtxHeaderParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public AtxHeaderParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get
            {
                return new[] { Opener };
            }
        }

        /// <summary>
        /// Gets the value indicating whether a handled element accepts new lines.
        /// </summary>
        /// <value><c>true</c> if new lines can be added to a handled element.</value>
        public override bool IsAcceptsLines
        {
            get { return true; }
        }

        /// <summary>
        /// Advances the offset and column values.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Advance(ref BlockParserInfo info)
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
            int offset;
            int headerLevel;
            if (!info.IsIndented && IsOpening(info) && 0 != (offset = ScanStart(info, out headerLevel)))
            {
                info.AdvanceOffset(info.FirstNonspace + offset - info.Offset, false);
                info.Container = CreateChildBlock(info, BlockTag.AtxHeader, info.FirstNonspace);
                info.Container.HeaderLevel = headerLevel;
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
            if (info.IsBlank)
            {
                AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace);
                return false;
            }

            AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace, TrimEnd(info));
            BlockMethods.Finalize(info.Container, info.LineInfo, Settings);
            info.Container = info.Container.Parent;
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
        /// Determines whether the current line can contain an ATX header.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the line can contain an ATX header.</returns>
        protected virtual bool IsOpening(BlockParserInfo info)
        {
            return true;
        }

        /// <summary>
        /// Gets the header opener character.
        /// </summary>
        /// <value>Opener character.</value>
        protected virtual char Opener
        {
            get { return '#'; }
        }

        /// <summary>
        /// Gets the header closer character.
        /// </summary>
        /// <value>Closer character.</value>
        protected virtual char Closer
        {
            get { return '#'; }
        }

        /// <summary>
        /// Matches ATX header start.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="headerLevel">Header level.</param>
        /// <returns>Offset, or 0 for no match.</returns>
        /// <remarks>Original: int scan_atx_header_start(string s, int pos, int sourceLength, out int headerLevel)</remarks>
        private int ScanStart(BlockParserInfo info, out int headerLevel)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            /*!re2c
              [#]{1,6} ([ ]+|[\n])  { return (p - start); }
              .? { return 0; }
            */

            headerLevel = 1;
            if (pos + 1 >= sourceLength)
                return 0;

            var o = Opener;
            var spaceExists = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (c == o)
                {
                    if (headerLevel == 6)
                        return 0;

                    if (spaceExists)
                        return i - pos;
                    else
                        headerLevel++;
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

        /// <summary>
        /// Trims ATX header end.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>Adjusted offset.</returns>
        private int TrimEnd(BlockParserInfo info)
        {
            int p = info.Line.Length - 1;

            // trim trailing spaces
            while (p >= 0 && (info.Line[p] == ' ' || info.Line[p] == '\n'))
                p--;

            // if string ends in #s, remove these:
            char c = Closer;
            while (p >= 0 && info.Line[p] == c)
                p--;

            // there must be a space before the last hashtag
            if (p < 0 || info.Line[p] != ' ')
                p = info.Line.Length - 1;

            return p - info.FirstNonspace + 1;
        }
    }
}
