﻿using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Link closer parser.
    /// </summary>
    public class LinkCloserParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkCloserParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public LinkCloserParser(CommonMarkSettings settings)
            : base(settings, ']')
        {
        }

        /// <summary>
        /// Parses a right bracket.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline HandleRightSquareBracket(Subject subj)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            var parameters = Settings.InlineParserParameters;

            // move past ']'
            subj.Position++;

            bool canClose;
            var istack = InlineStack.FindMatchingOpener(subj.LastPendingInline, InlineStack.InlineStackPriority.Links, '[', out canClose);

            if (istack != null)
            {
                // if the opener is "inactive" then it means that there was a nested link
                if (istack.DelimiterCount == -1)
                {
                    InlineStack.RemoveStackEntry(istack, subj, istack, parameters);
                    return new Inline("]", subj.Position - 1, subj.Position);
                }

                var endpos = subj.Position;

                // try parsing details for '[foo](/url "title")' or '[foo][bar]'
                var details = ParseLinkDetails(subj, parameters);

                // try lookup of the brackets themselves
                if (details == null || details == Reference.SelfReference)
                {
                    var startpos = istack.StartPosition;
                    var label = new StringPart(subj.Buffer, startpos, endpos - startpos - 1);

                    details = InlineMethods.LookupReference(subj.Document, label, parameters);
                }

                if (details == Reference.InvalidReference)
                    details = null;

                MatchSquareBracketStack(istack, subj, details, parameters);
                return null;
            }

            var inlText = new Inline("]", subj.Position - 1, subj.Position);

            if (canClose)
            {
                // note that the current implementation will not work if there are other inlines with priority
                // higher than Links.
                // to fix this the parsed link details should be added to the closer element in the stack.

                throw new NotSupportedException("It is not supported to have inline stack priority higher than Links.");

                ////AppendStackEntry(subj, inlText, InlineStack.InlineStackPriority.Links, InlineStack.InlineStackFlags.Closer, '[');
            }

            return inlText;
        }

        // Parse a link or the link portion of an image, or return a fallback.
        private static Reference ParseLinkDetails(Subject subj, InlineParserParameters parameters)
        {
            var endLabel = subj.Position;
            var curChar = InlineMethods.peek_char(subj);

            int spaceCount, urlLength;
            if (curChar == '(' &&
                    ((spaceCount = ScanSpaces(subj.Buffer, subj.Position + 1, subj.Length)) > -1) &&
                    ((urlLength = Scanner.scan_link_url(subj.Buffer, subj.Position + 1 + spaceCount, subj.Length)) > -1))
            {
                // try to parse an explicit link:
                var startUrl = subj.Position + 1 + spaceCount; // after (
                var endUrl = startUrl + urlLength;
                var startTitle = endUrl + ScanSpaces(subj.Buffer, endUrl, subj.Length);

                // ensure there are spaces btw url and title
                var endTitle = (startTitle == endUrl)
                    ? startTitle
                    : startTitle + Scanner.scan_link_title(subj.Buffer, startTitle, subj.Length);
                var endAll = endTitle + ScanSpaces(subj.Buffer, endTitle, subj.Length);
                if (endAll < subj.Length && subj.Buffer[endAll] == ')')
                {
                    subj.Position = endAll + 1;

                    var url = subj.Buffer.Substring(startUrl, urlLength);
                    url = InlineMethods.CleanUrl(url, parameters);
                    var title = subj.Buffer.Substring(startTitle, endTitle - startTitle);
                    title = InlineMethods.CleanTitle(title, parameters);

                    return new Reference(null, url, title);
                }
            }
            else if (curChar == '[')
            {
                var label = InlineMethods.ParseReferenceLabel(subj);
                if (label != null)
                {
                    if (label.Value.Length == 0)
                        return Reference.SelfReference;

                    var details = InlineMethods.LookupReference(subj.Document, label.Value, parameters);
                    if (details != null)
                        return details;

                    // rollback the subject but return InvalidReference so that the caller knows not to
                    // parse 'foo' from [foo][bar].
                    subj.Position = endLabel;
                    return Reference.InvalidReference;
                }
            }

            // rollback the subject position because didn't match anything.
            subj.Position = endLabel;
            return null;
        }

        private static void MatchSquareBracketStack(InlineStack opener, Subject subj, Reference details, InlineParserParameters parameters)
        {
            if (details != null)
            {
                var inl = opener.StartingInline;
                var isImage = 0 != (opener.Flags & InlineStack.InlineStackFlags.ImageLink);
                inl.Tag = isImage ? InlineTag.Image : InlineTag.Link;
                inl.FirstChild = inl.NextSibling;
                inl.NextSibling = null;
                inl.SourceLastPosition = subj.Position;

                inl.TargetUrl = details.Url;
                inl.LiteralContent = details.Title;

                if (!isImage)
                {
                    // since there cannot be nested links, remove any other link openers before this
                    var temp = opener.Previous;
                    while (temp != null && temp.Priority <= InlineStack.InlineStackPriority.Links)
                    {
                        if (temp.Delimiter == '[' && temp.Flags == opener.Flags)
                        {
                            // mark the previous entries as "inactive"
                            if (temp.DelimiterCount == -1)
                                break;

                            temp.DelimiterCount = -1;
                        }

                        temp = temp.Previous;
                    }
                }

                InlineStack.RemoveStackEntry(opener, subj, null, parameters);

                subj.LastInline = inl;
            }
            else
            {
                // this looked like a link, but was not.
                // remove the opener stack entry but leave the inbetween intact
                InlineStack.RemoveStackEntry(opener, subj, opener, parameters);

                var inl = new Inline("]", subj.Position - 1, subj.Position);
                subj.LastInline.LastSibling.NextSibling = inl;
                subj.LastInline = inl;
            }
        }

        /// <summary>
        /// Matches space characters, including newlines.
        /// </summary>
        /// <remarks>Original: int scan_spacechars(string s, int pos, int sourceLength)</remarks>
        private static int ScanSpaces(string s, int pos, int sourceLength)
        {
            /*!re2c
              [ \t\n]* { return (p - start); }
              . { return 0; }
            */

            if (pos >= sourceLength)
                return 0;

            for (var i = pos; i < sourceLength; i++)
            {
                if (!Utilities.IsWhitespace(s[i]))
                    return i - pos;
            }

            return sourceLength - pos;
        }
    }
}
