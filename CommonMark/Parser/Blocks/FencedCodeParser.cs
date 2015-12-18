using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.FencedCode"/> element parser.
    /// </summary>
    public class FencedCodeParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="opener">Opening character.</param>
        /// <param name="closer">Closing character. If unspecified, <paramref name="opener"/> will be used.</param>
        public FencedCodeParser(CommonMarkSettings settings, BlockTag tag, char opener, char closer = (char)0)
            : base(settings, tag, GetCharacters(opener, closer))
        {
            IsCodeBlock = true;
            IsAcceptsLines = true;

            // we don't count blanks in fenced code for purposes of tight/loose lists or breaking out of lists.
            IsAlwaysDiscardBlanks = true;

            Opener = opener;
            Closer = closer != 0 ? closer : opener;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            // -1 means we've seen closer 
            if (info.Container.FencedCodeData.FenceLength == -1)
            {
                if (info.IsBlank)
                    info.Container.IsLastLineBlank = true;
                return false;
            }

            // skip optional spaces of fence offset
            var i = info.Container.FencedCodeData.FenceOffset;
            while (i > 0 && info.Line[info.Offset] == ' ')
            {
                info.Offset++;
                info.Column++;
                i--;
            }

            return true;
        }

        /// <summary>
        /// Opens a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Open(ref BlockParserInfo info)
        {
            int fenceLength;
            if (!info.IsIndented && 0 != (fenceLength = ScanOpening(info)))
            {
                info.Container = CreateChildBlock(info, Tag, info.FirstNonspace);
                info.Container.FencedCodeData = new FencedCodeData
                {
                    FenceChar = info.CurrentCharacter,
                    FenceLength = fenceLength,
                    FenceOffset = info.FirstNonspace - info.Offset,
                };
                info.AdvanceOffset(info.FirstNonspace + fenceLength - info.Offset, false);
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
            if (!info.IsIndented && IsClosing(info) && 0 != ScanClosing(info))
            {
                // if closing fence, set fence length to -1. it will be closed when the next line is processed. 
                info.Container.FencedCodeData.FenceLength = -1;
            }
            else
            {
                AddLine(info.Container, info.LineInfo, info.Line, info.Offset);
            }
            return true;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            // first line of contents becomes info
            var firstlinelen = container.StringContent.IndexOf('\n') + 1;
            container.FencedCodeData.Info = InlineMethods.Unescape(container.StringContent.TakeFromStart(firstlinelen, true).Trim());
            return true;
        }

        /// <summary>
        /// Determines whether the current line can serve as a closing fence.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the line can be a closing fence.</returns>
        private bool IsClosing(BlockParserInfo info)
        {
            return Opener != Closer || info.CurrentCharacter == info.Container.FencedCodeData.FenceChar;
        }

        /// <summary>
        /// Scans an opening code fence.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>The number of characters forming the fence, or 0 for no match.</returns>
        /// <remarks>Original: int scan_open_code_fence(string s, int pos, int sourceLength, BlockParserParameters parameters)</remarks>
        private int ScanOpening(BlockParserInfo info)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var sourceLength = s.Length;

            /*!re2c
              [`]{3,} / [^`\n\x00]*[\n] { return (p - start); }
              [~]{3,} / [^~\n\x00]*[\n] { return (p - start); }
              .?                        { return 0; }
            */

            if (pos + 3 >= sourceLength)
                return 0;

            var cnt = 1;
            var fenceDone = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (c == Opener)
                {
                    if (fenceDone)
                        return 0;

                    cnt++;
                    continue;
                }

                fenceDone = true;
                if (cnt < 3)
                    return 0;

                if (c == '\n')
                    return cnt;
            }

            if (cnt < 3)
                return 0;

            return cnt;
        }

        /// <summary>
        /// Scan a closing code fence with length at least <see cref="FencedCodeData.FenceLength"/>.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns>The number of characters forming the fence, or 0 for no match.</returns>
        /// <remarks>Original: int scan_close_code_fence(string s, int pos, int len, int sourceLength, BlockParserParameters parameters)</remarks>
        private int ScanClosing(BlockParserInfo info)
        {
            var s = info.Line;
            var pos = info.FirstNonspace;
            var len = info.Container.FencedCodeData.FenceLength;
            var sourceLength = s.Length;

            /*!re2c
              ([`]{3,} | [~]{3,}) / spacechar* [\n]
                                          { if (p - start > len) {
                                            return (p - start);
                                          } else {
                                            return 0;
                                          } }
              .? { return 0; }
            */

            if (pos + len >= sourceLength)
                return 0;

            var c1 = s[pos];

            var cnt = 1;
            var spaces = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == c1 && !spaces)
                    cnt++;
                else if (c == ' ')
                    spaces = true;
                else if (c == '\n')
                    return cnt < len ? 0 : cnt;
                else
                    return 0;
            }

            return 0;
        }

        /// <summary>
        /// Gets the fence opener character.
        /// </summary>
        /// <value>Opener character.</value>
        private char Opener
        {
            get;
        }

        /// <summary>
        /// Gets the fence closer character.
        /// </summary>
        /// <value>Closer character.</value>
        private char Closer
        {
            get;
        }

        private static char[] GetCharacters(char opener, char closer)
        {
            return opener != closer && closer != 0
                ? new[] { opener, closer }
                : new[] { opener };
        }
    }
}
