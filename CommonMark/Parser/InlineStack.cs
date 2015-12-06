using System;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Describes an element in a stack of possible inline openers.
    /// </summary>
    public sealed class InlineStack
    {
        /// <summary>
        /// The parser priority if this stack entry.
        /// </summary>
        public InlineStackPriority Priority;

        /// <summary>
        /// Previous entry in the stack. <c>null</c> if this is the last one.
        /// </summary>
        public InlineStack Previous;

        /// <summary>
        /// Next entry in the stack. <c>null</c> if this is the last one.
        /// </summary>
        public InlineStack Next;

        /// <summary>
        /// The at-the-moment text inline that could be transformed into the opener.
        /// </summary>
        public Inline StartingInline;

        /// <summary>
        /// The number of delimeter characters found for this opener.
        /// </summary>
        public int DelimeterCount;

        /// <summary>
        /// The character that was used in the opener.
        /// </summary>
        public char Delimeter;

        /// <summary>
        /// The position in the <see cref="Subject.Buffer"/> where this inline element was found.
        /// Used only if the specific parser requires this information.
        /// </summary>
        public int StartPosition;

        /// <summary>
        /// The flags set for this stack entry.
        /// </summary>
        public InlineStackFlags Flags;

        /// <summary>
        /// Inline stack element flags.
        /// </summary>
        [Flags]
        public enum InlineStackFlags : byte
        {
            /// <summary>
            /// The element has no flags.
            /// </summary>
            None = 0,

            /// <summary>
            /// The element is a stack opener.
            /// </summary>
            Opener = 1,

            /// <summary>
            /// The element is a stack closer.
            /// </summary>
            Closer = 2,

            /// <summary>
            /// The element is an image link.
            /// </summary>
            ImageLink = 4
        }

        /// <summary>
        /// Inline stack priority.
        /// </summary>
        public enum InlineStackPriority : byte
        {
            /// <summary>
            /// Emphasis elements.
            /// </summary>
            Emphasis = 0,

            /// <summary>
            /// Link elements.
            /// </summary>
            Links = 1,

            /// <summary>
            /// Maximum priority value.
            /// </summary>
            Maximum = Links
        }

        /// <summary>
        /// Attempts to match an opening element to the specified closing element.
        /// </summary>
        /// <param name="seachBackwardsFrom">Stack element to search backwards from.</param>
        /// <param name="priority">Priority.</param>
        /// <param name="delimeter">Delimiter character.</param>
        /// <param name="canClose"><c>true</c> if a matching opener was found.</param>
        /// <returns></returns>
        public static InlineStack FindMatchingOpener(InlineStack seachBackwardsFrom, InlineStackPriority priority, char delimeter, out bool canClose)
        {
            canClose = true;
            var istack = seachBackwardsFrom;
            while (true)
            {
                if (istack == null)
                {
                    // this cannot be a closer since there is no opener available.
                    canClose = false;
                    return null;
                }

                if (istack.Priority > priority ||
                    (istack.Delimeter == delimeter && 0 != (istack.Flags & InlineStackFlags.Closer)))
                {
                    // there might be a closer further back but we cannot go there yet because a higher priority element is blocking
                    // the other option is that the stack entry could be a closer for the same char - this means
                    // that any opener we might find would first have to be matched against this closer.
                    return null;
                }

                if (istack.Delimeter == delimeter)
                    return istack;

                istack = istack.Previous;
            }
        }

        /// <summary>
        /// Appends an element to the stack.
        /// </summary>
        /// <param name="entry">Stack element to append.</param>
        /// <param name="subj">Subject.</param>
        public static void AppendStackEntry(InlineStack entry, Subject subj)
        {
            if (subj.LastPendingInline != null)
            {
                entry.Previous = subj.LastPendingInline;
                subj.LastPendingInline.Next = entry;
            }

            if (subj.FirstPendingInline == null)
                subj.FirstPendingInline = entry;

            subj.LastPendingInline = entry;
        }

        /// <summary>
        /// Removes a subset of the stack.
        /// </summary>
        /// <param name="first">The first entry to be removed.</param>
        /// <param name="subj">The subject associated with this stack. Can be <c>null</c> if the pointers in the subject should not be updated.</param>
        /// <param name="last">The last entry to be removed. Can be <c>null</c> if everything starting from <paramref name="first"/> has to be removed.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        public static void RemoveStackEntry(InlineStack first, Subject subj, InlineStack last, InlineParserParameters parameters)
        {
            var curPriority = first.Priority;

            if (last == null)
            {
                if (first.Previous != null)
                    first.Previous.Next = null;
                else if (subj != null)
                    subj.FirstPendingInline = null;

                if (subj != null)
                {
                    last = subj.LastPendingInline;
                    subj.LastPendingInline = first.Previous;
                }

                first = first.Next;
            }
            else
            {
                if (first.Previous != null)
                    first.Previous.Next = last.Next;
                else if (subj != null)
                    subj.FirstPendingInline = last.Next;

                if (last.Next != null)
                    last.Next.Previous = first.Previous;
                else if (subj != null)
                    subj.LastPendingInline = first.Previous;

                if (first == last)
                    return;

                first = first.Next;
                last = last.Previous;
            }

            if (last == null || first == null)
                return;

            first.Previous = null;
            last.Next = null;

            // handle case like [*b*] (the whole [..] is being removed but the inner *..* must still be matched).
            // this is not done automatically because the initial * is recognized as a potential closer (assuming
            // potential scenario '*[*' ).
            if (curPriority > 0)
                PostProcessInlineStack(null, first, last, curPriority, parameters);
        }

        /// <summary>
        /// Post-processes the stack.
        /// </summary>
        /// <param name="subj">Subject.</param>
        /// <param name="first">First stack element.</param>
        /// <param name="last">Last stack element.</param>
        /// <param name="ignorePriority">Initial priority.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        public static void PostProcessInlineStack(Subject subj, InlineStack first, InlineStack last, InlineStackPriority ignorePriority, InlineParserParameters parameters)
        {
            var singleCharTags = parameters.SingleCharTags;
            var doubleCharTags = parameters.DoubleCharTags;
            while (ignorePriority > 0)
            {
                var istack = first;
                while (istack != null)
                {
                    if (istack.Priority >= ignorePriority)
                    {
                        RemoveStackEntry(istack, subj, istack, parameters);
                    }
                    else if (0 != (istack.Flags & InlineStackFlags.Closer))
                    {
                        bool canClose;
                        var iopener = FindMatchingOpener(istack.Previous, istack.Priority, istack.Delimeter, out canClose);
                        if (iopener != null)
                        {
                            bool retry = false;
                            var singleCharTag = iopener.Delimeter < singleCharTags.Length ? singleCharTags[iopener.Delimeter] : 0;
                            var doubleCharTag = iopener.Delimeter < doubleCharTags.Length ? doubleCharTags[iopener.Delimeter] : 0;
                            if (singleCharTag != 0 || doubleCharTag != 0)
                            {
                                var useDelims = InlineMethods.MatchInlineStack(iopener, subj, istack.DelimeterCount, istack, singleCharTag, doubleCharTag, parameters);
                                if (istack.DelimeterCount > 0)
                                    retry = true;
                            }

                            if (retry)
                            {
                                // remove everything between opened and closer (not inclusive).
                                if (istack.Previous != null && iopener.Next != istack.Previous)
                                    RemoveStackEntry(iopener.Next, subj, istack.Previous, parameters);

                                continue;
                            }
                            else
                            {
                                // remove opener, everything in between, and the closer
                                RemoveStackEntry(iopener, subj, istack, parameters);
                            }
                        }
                        else if (!canClose)
                        {
                            // this case means that a matching opener does not exist
                            // remove the Closer flag so that a future Opener can be matched against it.
                            istack.Flags &= ~InlineStackFlags.Closer;
                        }
                    }

                    if (istack == last)
                        break;

                    istack = istack.Next;
                }

                ignorePriority--;
            }
        }
    }
}
