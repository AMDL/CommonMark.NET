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
                return new[] { '#' };
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
            int matched;
            int i;
            if (!info.IsIndented && 0 != (matched = Scanner.scan_atx_header_start(info.Line, info.FirstNonspace, info.Line.Length, out i)))
            {
                info.AdvanceOffset(info.FirstNonspace + matched - info.Offset, false);
                info.Container = CreateChildBlock(info, BlockTag.AtxHeader, info.FirstNonspace);
                info.Container.HeaderLevel = i;
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

            int p = info.Line.Length - 1;

            // trim trailing spaces
            while (p >= 0 && (info.Line[p] == ' ' || info.Line[p] == '\n'))
                p--;

            // if string ends in #s, remove these:
            while (p >= 0 && info.Line[p] == '#')
                p--;

            // there must be a space before the last hashtag
            if (p < 0 || info.Line[p] != ' ')
                p = info.Line.Length - 1;

            AddLine(info.Container, info.LineInfo, info.Line, info.FirstNonspace, p - info.FirstNonspace + 1);
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
    }
}
