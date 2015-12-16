using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Emphasis parser.
    /// </summary>
    public class EmphasisParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisParser"/> class.
        /// </summary>
        /// <param name="singleCharTag">Single-character delimiter parameters.</param>
        /// <param name="doubleCharTag">Double-character delimiter parameters.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <param name="c">Handled character.</param>
        public EmphasisParser(InlineTag singleCharTag, InlineTag doubleCharTag, InlineParserParameters parameters, char c)
            : base(parameters.Settings, c)
        {
            this.Delimiters = InitializeDelimiters(singleCharTag, doubleCharTag);
            this.Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisParser"/> class.
        /// </summary>
        /// <param name="delimiters">Inline delimiter character parameters.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <param name="c">Handled character.</param>
        public EmphasisParser(InlineDelimiterCharacterParameters delimiters, InlineParserParameters parameters, char c)
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

        /// <summary>
        /// Attempts to match a stack delimiter.
        /// </summary>
        /// <param name="subj">The source subject.</param>
        /// <param name="startpos">The index of the first character.</param>
        /// <param name="length">The length of the substring.</param>
        /// <param name="beforeIsPunctuation"><c>true</c> if the substring is preceded by a punctuation character.</param>
        /// <param name="afterIsPunctuation"><c>true</c> if the substring is followed by a punctuation character.</param>
        /// <param name="canOpen"><c>true</c> if the delimiter can serve as an opener.</param>
        /// <param name="canClose"><c>true</c> if the delimiter can serve as a closer.</param>
        protected virtual void Match(Subject subj, int startpos, int length, bool beforeIsPunctuation, bool afterIsPunctuation, ref bool canOpen, ref bool canClose)
        {
        }

        private InlineDelimiterCharacterParameters Delimiters { get; }

        private InlineParserParameters Parameters { get; }

        private InlineDelimiterCharacterParameters InitializeDelimiters(InlineTag singleCharTag, InlineTag doubleCharTag)
        {
            return new InlineDelimiterCharacterParameters
            {
                SingleCharacter = InitializeDelimiter(singleCharTag),
                DoubleCharacter = InitializeDelimiter(doubleCharTag),
            };
        }

        private InlineDelimiterParameters InitializeDelimiter(InlineTag tag)
        {
            return new InlineDelimiterParameters
            {
                Tag = tag,
                Matcher = Match,
            };
        }

        /// <summary>
        /// Scans the subject for a series of the given emphasis character, testing if they could open and/or close
        /// an emphasis element.
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

            char charBefore, charAfter;
            bool beforeIsSpace, beforeIsPunctuation, afterIsSpace, afterIsPunctuation;

            charBefore = startpos == 0 ? '\n' : subj.Buffer[startpos - 1];
            subj.Position = (startpos += numdelims);
            charAfter = len == startpos ? '\n' : subj.Buffer[startpos];

            Utilities.CheckUnicodeCategory(charBefore, out beforeIsSpace, out beforeIsPunctuation);
            Utilities.CheckUnicodeCategory(charAfter, out afterIsSpace, out afterIsPunctuation);

            canOpen = !afterIsSpace && !(afterIsPunctuation && !beforeIsSpace && !beforeIsPunctuation);
            canClose = !beforeIsSpace && !(beforeIsPunctuation && !afterIsSpace && !afterIsPunctuation);

            var matcher = numdelims == 1
                ? Delimiters.SingleCharacter.Matcher
                : Delimiters.DoubleCharacter.Matcher;
            if (matcher != null)
            {
                matcher(subj, startpos, len, beforeIsPunctuation, afterIsPunctuation, ref canOpen, ref canClose);
            }

            return numdelims;
        }
    }
}
