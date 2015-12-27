using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Fenced code delimiter parameters.
    /// </summary>
    public sealed class FencedCodeDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeDelimiterParameters"/> class.
        /// </summary>
        /// <param name="opener">Opening character.</param>
        /// <param name="closer">Closing character. If unspecified, <paramref name="opener"/> will be used.</param>
        public FencedCodeDelimiterParameters(char opener, char closer = (char)0)
        {
            Opener = opener;
            Closer = closer != 0 ? closer : opener;
        }

        /// <summary>
        /// Gets or sets the fence opener character.
        /// </summary>
        /// <value>Opener character.</value>
        public char Opener { get; set; }

        /// <summary>
        /// Gets or sets the fence closer character.
        /// </summary>
        /// <value>Closer character.</value>
        public char Closer { get; set; }
    }

    /// <summary>
    /// Fenced code delimiter handler.
    /// </summary>
    public sealed class FencedCodeHandler : BlockDelimiterHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDelimiterHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="parameters">Fenced code delimiter parameters.</param>
        public FencedCodeHandler(CommonMarkSettings settings, BlockTag tag, FencedCodeDelimiterParameters parameters)
            : base(settings, tag, parameters.Opener)
        {
            Opener = parameters.Opener;
        }

        /// <summary>
        /// Handles a fenced code block opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            int fenceLength;
            if (!info.IsIndented && 0 != (fenceLength = ScanOpening(info)))
            {
                info.Container = AppendChildBlock(info, Tag, info.FirstNonspace);
                info.Container.FencedCode = new FencedCodeData
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

        private char Opener
        {
            get;
        }
    }
}
