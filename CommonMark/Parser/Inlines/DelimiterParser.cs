using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Delimiter parser.
    /// </summary>
    internal class DelimiterParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelimiterParser"/> class.
        /// </summary>
        /// <param name="delimiters">Inline delimiter character parameters.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <param name="c">Handled character.</param>
        public DelimiterParser(InlineDelimiterCharacterParameters delimiters, InlineParserParameters parameters, char c)
            : base(parameters.Settings, c)
        {
            this.Delimiters = delimiters;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Handles an emphasis opener.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline HandleOpenerCloser(Subject subj, InlineTag singleCharTag, InlineTag doubleCharTag)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            bool canOpen, canClose;
            var numdelims = Scan(subj, out canOpen, out canClose);

            if (canClose)
            {
                // walk the stack and find a matching opener, if there is one
                var istack = InlineStack.FindMatchingOpener(subj.LastPendingInline, InlineStack.InlineStackPriority.Emphasis, Character, out canClose);
                if (istack != null)
                {
                    var useDelims = InlineMethods.MatchInlineStack(istack, subj, numdelims, null, Delimiters, Parameters);

                    // if the closer was not fully used, move back a char or two and try again.
                    if (useDelims < numdelims)
                    {
                        subj.Position = subj.Position - numdelims + useDelims;

                        // use recursion only if it will not be very deep.
                        if (numdelims < 10)
                            return Handle(container, subj);
                    }

                    return null;
                }
            }

            var inlText = new Inline(subj.Buffer, subj.Position - numdelims, numdelims, subj.Position - numdelims, subj.Position, Character);

            if (canOpen || canClose)
            {
                var flags = (canOpen ? InlineStack.InlineStackFlags.Opener : 0)
                        | (canClose ? InlineStack.InlineStackFlags.Closer : 0);
                AppendStackEntry(subj, inlText, InlineStack.InlineStackPriority.Emphasis, flags, Character, numdelims);
            }

            return inlText;
        }

        private InlineDelimiterCharacterParameters Delimiters { get; }

        private InlineParserParameters Parameters { get; }

        /// <summary>
        /// Scans the subject for a series of the given character, testing if they could open and/or close a handled element.
        /// </summary>
        private int Scan(Subject subj, out bool canOpen, out bool canClose)
        {
            int numdelims = 0;
            int startpos = subj.Position;
            int len = subj.Length;

            while (startpos + numdelims < len && subj.Buffer[startpos + numdelims] == Character)
                numdelims++;

            if (numdelims == 0)
            {
                canOpen = false;
                canClose = false;
                return numdelims;
            }

            var charBefore = startpos == 0 ? '\n' : subj.Buffer[startpos - 1];
            var before = Utilities.GetCharacterType(charBefore);

            subj.Position = (startpos += numdelims);

            var charAfter = len == startpos ? '\n' : subj.Buffer[startpos];
            var after = Utilities.GetCharacterType(charAfter);

            canOpen = !after.IsSpace && !(after.IsPunctuation && !before.IsSpace && !before.IsPunctuation);
            canClose = !before.IsSpace && !(before.IsPunctuation && !after.IsSpace && !after.IsPunctuation);

            var delimParams = numdelims == 1
                ? Delimiters.SingleCharacter
                : Delimiters.DoubleCharacter;

            if (delimParams.Handler != null)
            {
                delimParams.Handler(subj, startpos, len, before, after, ref canOpen, ref canClose);
            }

            return numdelims;
        }
    }
}
