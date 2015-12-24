using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// ATX heading parameters.
    /// </summary>
    public sealed class AtxHeadingParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtxHeadingParameters"/> class.
        /// </summary>
        /// <param name="opener">Opening character.</param>
        /// <param name="closer">Closing character. If unspecified, <paramref name="opener"/> will be used.</param>
        public AtxHeadingParameters(char opener, char closer = (char)0)
        {
            Opener = opener;
            Closer = closer != 0 ? closer : opener;
        }

        /// <summary>
        /// Gets or sets the heading opener character.
        /// </summary>
        /// <value>Opener character.</value>
        public char Opener { get; set; }

        /// <summary>
        /// Gets or sets the heading closer character.
        /// </summary>
        /// <value>Closer character.</value>
        public char Closer { get; set; }
    }

    /// <summary>
    /// <see cref="BlockTag.AtxHeading"/> element parser.
    /// </summary>
    public sealed class AtxHeadingParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly AtxHeadingParameters DefaultParameters = new AtxHeadingParameters('#');

        /// <summary>
        /// Initializes a new instance of the <see cref="AtxHeadingParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parameters">ATX heading parameters.</param>
        public AtxHeadingParser(CommonMarkSettings settings, BlockTag tag = BlockTag.AtxHeading, AtxHeadingParameters parameters = null)
            : base(settings, tag)
        {
            IsAcceptsLines = true;

            parameters = parameters ?? DefaultParameters;
            Opener = parameters.Opener;
            Closer = parameters.Closer;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                yield return new Delimiters.AtxHeadingHandler(Settings, Tag, Opener);
            }
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            // a heading can never contain more than one line
            if (info.IsBlank)
            {
                info.Container.IsLastLineBlank = true;
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
        /// Trims an ATX heading end.
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
            while (p >= 0 && info.Line[p] == Closer)
                p--;

            // there must be a space before the last hashtag
            if (p < 0 || info.Line[p] != ' ')
                p = info.Line.Length - 1;

            return p - info.FirstNonspace + 1;
        }

        private char Opener
        {
            get;
        }

        private char Closer
        {
            get;
        }
    }
}
