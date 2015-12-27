using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Fenced code parameters.
    /// </summary>
    public sealed class FencedCodeParameters
    {
        /// <summary>
        /// Gets or sets the fenced code delimiter parameters.
        /// </summary>
        public FencedCodeDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.FencedCode"/> element parser.
    /// </summary>
    public sealed class FencedCodeParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly FencedCodeParameters DefaultParameters = new FencedCodeParameters
        {
            Delimiters = new[]
            {
                new FencedCodeDelimiterParameters('`'),
                new FencedCodeDelimiterParameters('~'),
            },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parameters">Fenced code parameters.</param>
        public FencedCodeParser(CommonMarkSettings settings, BlockTag tag = BlockTag.FencedCode, FencedCodeParameters parameters = null)
            : base(settings, tag)
        {
            IsCodeBlock = true;
            IsAcceptsLines = true;

            // we don't count blanks in fenced code for purposes of tight/loose lists or breaking out of lists.
            IsAlwaysDiscardBlanks = true;

            Parameters = parameters ?? DefaultParameters;
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
                    yield return new FencedCodeHandler(Settings, Tag, delimiter);
                }
            }
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            // -1 means we've seen closer 
            if (info.Container.FencedCode.FenceLength == -1)
            {
                if (info.IsBlank)
                    info.Container.IsLastLineBlank = true;
                return false;
            }

            // skip optional spaces of fence offset
            var i = info.Container.FencedCode.FenceOffset;
            while (i > 0 && info.Line[info.Offset] == ' ')
            {
                info.Offset++;
                info.Column++;
                i--;
            }

            return true;
        }

        /// <summary>
        /// Closes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Close(BlockParserInfo info)
        {
            if (!info.IsIndented && CanClose(info) && 0 != ScanClosing(info))
            {
                // if closing fence, set fence length to -1. it will be closed when the next line is processed. 
                info.Container.FencedCode.FenceLength = -1;
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
            container.FencedCode.Info = InlineMethods.Unescape(container.StringContent.TakeFromStart(firstlinelen, true).Trim(), Settings.InlineParserParameters);
            return true;
        }

        /// <summary>
        /// Determines whether the current line can serve as a closing fence.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the line can be a closing fence.</returns>
        private bool CanClose(BlockParserInfo info)
        {
            return Parameters.Delimiters.Length == 1
                ? info.CurrentCharacter == Parameters.Delimiters[0].Opener
                : info.CurrentCharacter == info.Container.FencedCode.FenceChar;
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
            var len = info.Container.FencedCode.FenceLength;
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

        private FencedCodeParameters Parameters
        {
            get;
        }
    }
}
