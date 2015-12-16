using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal static class InlineMethods
    {
        private static readonly char[] WhiteSpaceCharacters = new[] { '\n', ' ' };
        private static readonly char[] BracketSpecialCharacters = new[] { '\\', ']', '[' };

        /// <summary>
        /// Collapses internal whitespace to single space, removes leading/trailing whitespace, folds case.
        /// </summary>
        private static string NormalizeReference(StringPart s, InlineParserParameters parameters)
        {
            if (s.Length == 0)
                return string.Empty;

            var result = NormalizeWhitespace(s.Source, s.StartIndex, s.Length);
            return parameters.ReferenceNormalizer(result);
        }

        /// <summary>
        /// Looks up a reference with the given label in the reference dictionary.
        /// </summary>
        /// <returns>
        /// A valid reference, <see cref="Reference.InvalidReference"/> if the reference label is not valid, or <c>null</c>.
        /// </returns>
        public static Reference LookupReference(Dictionary<string, Reference> refmap, StringPart lab, InlineParserParameters parameters)
        {
            if (refmap == null)
                return null;

            if (lab.Length > Reference.MaximumReferenceLabelLength)
                return Reference.InvalidReference;

            string label = NormalizeReference(lab, parameters);

            Reference r;
            if (refmap.TryGetValue(label, out r))
                return r;

            return null;
        }

        /// <summary>
        /// Adds a new reference to the dictionary, if the label does not already exist there.
        /// Assumes that the length of the label does not exceed <see cref="Reference.MaximumReferenceLabelLength"/>.
        /// </summary>
        private static void AddReference(Dictionary<string, Reference> refmap, StringPart label, string url, string title, InlineParserParameters parameters)
        {
            var normalizedLabel = NormalizeReference(label, parameters);
            if (refmap.ContainsKey(normalizedLabel))
                return;

            refmap.Add(normalizedLabel, new Reference(normalizedLabel, url, title));
        }

        // Return the next character in the subject, without advancing.
        // Return 0 if at the end of the subject.
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static char peek_char(Subject subj)
        {
            return subj.Length <= subj.Position ? '\0' : subj.Buffer[subj.Position];
        }

        /// <summary>
        /// Collapses consecutive space and newline characters into a single space.
        /// Additionaly removes leading and trailing spaces.
        /// </summary>
        internal static string NormalizeWhitespace(string s, int startIndex, int count)
        {
            char c;

            // count will actually be the lastIndex. The method argument is count only because other similar methods have startIndex/count
            count = startIndex + count - 1;

            // trim leading and trailing spaces.
            while (startIndex < count)
            {
                c = s[startIndex];
                if (c != ' ' && c != '\n') break;
                startIndex++;
            }

            while (count >= startIndex)
            {
                c = s[count];
                if (c != ' ' && c != '\n') break;
                count--;
            }

            if (count < startIndex)
                return string.Empty;

            // collapse inner whitespace
            // the complexity of this method is mainly so that the use of StringBuilder could be avoided if it is not needed
            StringBuilder sb = null;
            int pos = startIndex;
            int lastPos = startIndex;
            while (-1 != (pos = s.IndexOfAny(WhiteSpaceCharacters, pos, count - pos)))
            {
                if (s[pos] == '\n')
                {
                    if (sb == null)
                        sb = new StringBuilder(s.Length);

                    // newline has to be replaced with ' '
                    sb.Append(s, lastPos, pos - lastPos);
                    sb.Append(' ');

                    // move past consecutive spaces
                    do
                    {
                        c = s[++pos];
                        if (c != ' ' && c != '\n')
                            break;
                    } while (pos < count);

                    lastPos = pos;
                }
                else
                {
                    c = s[++pos];

                    if (c == ' ' || c == '\n')
                    {
                        // multiple consecutive whitespaces
                        if (sb == null)
                            sb = new StringBuilder(s.Length);

                        sb.Append(s, lastPos, pos - lastPos);

                        // move past consecutive spaces
                        do
                        {
                            c = s[++pos];
                            if (c != ' ' && c != '\n')
                                break;
                        } while (pos < count);

                        lastPos = pos;
                    }
                }
            }

            if (sb == null)
                return s.Substring(startIndex, count - startIndex + 1);

            sb.Append(s, lastPos, count - lastPos + 1);
            return sb.ToString();
        }

        internal static int MatchInlineStack(InlineStack opener, Subject subj, int closingDelimeterCount, InlineStack closer, InlineDelimiterCharacterParameters delimiters, InlineParserParameters parameters)
        {
            // calculate the actual number of delimeters used from this closer
            int useDelims;
            var openerDelims = opener.DelimeterCount;

            var singleChar = delimiters.SingleCharacter;
            var doubleChar = delimiters.DoubleCharacter;
            if (closingDelimeterCount < 3 || openerDelims < 3)
            {
                useDelims = closingDelimeterCount <= openerDelims ? closingDelimeterCount : openerDelims;
                if (useDelims == 2 && doubleChar.IsEmpty)
                    useDelims = 1;
                if (useDelims == 1 && singleChar.IsEmpty)
                    return 0;
            }
            else if (singleChar.IsEmpty)
                useDelims = 2;
            else if (doubleChar.IsEmpty)
                useDelims = 1;
            else
                useDelims = closingDelimeterCount % 2 == 0 ? 2 : 1;

            Inline inl = opener.StartingInline;
            InlineTag tag = useDelims == 1 ? singleChar.Tag : doubleChar.Tag;
            if (openerDelims == useDelims)
            {
                // the opener is completely used up - remove the stack entry and reuse the inline element
                inl.Tag = tag;
                inl.LiteralContent = null;
                inl.FirstChild = inl.NextSibling;
                inl.NextSibling = null;

                InlineStack.RemoveStackEntry(opener, subj, closer?.Previous, parameters);
            }
            else
            {
                // the opener will only partially be used - stack entry remains (truncated) and a new inline is added.
                opener.DelimeterCount -= useDelims;
                inl.LiteralContent = inl.LiteralContent.Substring(0, opener.DelimeterCount);
                inl.SourceLastPosition -= useDelims;

                inl.NextSibling = new Inline(tag, inl.NextSibling);
                inl = inl.NextSibling;

                inl.SourcePosition = opener.StartingInline.SourcePosition + opener.DelimeterCount;
            }

            // there are two callers for this method, distinguished by the `closer` argument.
            // if closer == null it means the method is called during the initial subject parsing and the closer
            //   characters are at the current position in the subject. The main benefit is that there is nothing
            //   parsed that is located after the matched inline element.
            // if closer != null it means the method is called when the second pass for previously unmatched
            //   stack elements is done. The drawback is that there can be other elements after the closer.
            if (closer != null)
            {
                var clInl = closer.StartingInline;
                if ((closer.DelimeterCount -= useDelims) > 0)
                {
                    // a new inline element must be created because the old one has to be the one that
                    // finalizes the children of the emphasis
                    var newCloserInline = new Inline(clInl.LiteralContent.Substring(useDelims));
                    newCloserInline.SourcePosition = inl.SourceLastPosition = clInl.SourcePosition + useDelims;
                    newCloserInline.SourceLength = closer.DelimeterCount;
                    newCloserInline.NextSibling = clInl.NextSibling;

                    clInl.LiteralContent = null;
                    clInl.NextSibling = null;
                    inl.NextSibling = closer.StartingInline = newCloserInline;
                }
                else
                {
                    inl.SourceLastPosition = clInl.SourceLastPosition;

                    clInl.LiteralContent = null;
                    inl.NextSibling = clInl.NextSibling;
                    clInl.NextSibling = null;
                }
            }
            else if (subj != null)
            {
                inl.SourceLastPosition = subj.Position - closingDelimeterCount + useDelims;
                subj.LastInline = inl;
            }

            return useDelims;
        }

        /// <summary>
        /// Destructively unescape a string: remove backslashes before punctuation or symbol characters.
        /// </summary>
        /// <param name="url">The string data that will be changed by unescaping any punctuation or symbol characters.</param>
        public static string Unescape(string url)
        {
            // remove backslashes before punctuation chars:
            int searchPos = 0;
            int lastPos = 0;
            int match;
            char c;
            char[] search = new[] { '\\', '&' };
            StringBuilder sb = null;

            while ((searchPos = url.IndexOfAny(search, searchPos)) != -1)
            {
                c = url[searchPos];
                if (c == '\\')
                {
                    searchPos++;

                    if (url.Length == searchPos)
                        break;

                    c = url[searchPos];
                    if (Utilities.IsEscapableSymbol(c))
                    {
                        if (sb == null) sb = new StringBuilder(url.Length);
                        sb.Append(url, lastPos, searchPos - lastPos - 1);
                        lastPos = searchPos;
                    }
                }
                else if (c == '&')
                {
                    string namedEntity;
                    int numericEntity;
                    match = Scanner.scan_entity(url, searchPos, url.Length - searchPos, out namedEntity, out numericEntity);
                    if (match == 0)
                    {
                        searchPos++;
                    }
                    else
                    {
                        searchPos += match;

                        if (namedEntity != null)
                        {
                            var decoded = EntityDecoder.DecodeEntity(namedEntity);
                            if (decoded != null)
                            {
                                if (sb == null) sb = new StringBuilder(url.Length);
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                                lastPos = searchPos;
                            }
                        }
                        else if (numericEntity > 0)
                        {
                            var decoded = EntityDecoder.DecodeEntity(numericEntity);
                            if (decoded != null)
                            {
                                if (sb == null) sb = new StringBuilder(url.Length);
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                            }
                            else
                            {
                                if (sb == null) sb = new StringBuilder(url.Length);
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append('\uFFFD');
                            }

                            lastPos = searchPos;
                        }
                    }
                }
            }

            if (sb == null)
                return url;

            sb.Append(url, lastPos, url.Length - lastPos);
            return sb.ToString();
        }

        /// <summary>
        /// Clean a URL: remove surrounding whitespace and surrounding &lt; &gt; and remove \ that escape punctuation and other symbols.
        /// </summary>
        /// <remarks>Original: clean_url(ref string)</remarks>
        public static string CleanUrl(string url)
        {
            if (url.Length == 0)
                return url;

            // remove surrounding <> if any:
            url = url.Trim();

            if (url[0] == '<' && url[url.Length - 1] == '>')
                url = url.Substring(1, url.Length - 2);

            return Unescape(url);
        }

        /// <summary>
        /// Clean a title: remove surrounding quotes and remove \ that escape punctuation.
        /// </summary>
        /// <remarks>Original: clean_title(ref string)</remarks>
        internal static string CleanTitle(string title)
        {
            // remove surrounding quotes if any:
            int titlelength = title.Length;
            if (titlelength == 0)
                return title;

            var a = title[0];
            var b = title[titlelength - 1];
            if ((a == '\'' && b == '\'') || (a == '(' && b == ')') || (a == '"' && b == '"'))
                title = title.Substring(1, titlelength - 2);

            return Unescape(title);
        }

        // Parse zero or more space characters, including at most one newline.
        private static void spnl(Subject subj)
        {
            var seenNewline = false;
            var len = subj.Length;
            while (subj.Position < len)
            {
                var c = subj.Buffer[subj.Position];
                if (c == ' ' || (!seenNewline && (seenNewline = c == '\n')))
                    subj.Position++;
                else
                    return;
            }
        }

        /// <summary>
        /// Parses the contents of [..] for a reference label. Only used for parsing 
        /// reference definition labels for use with the reference dictionary because 
        /// it does not properly parse nested inlines.
        /// 
        /// Assumes the source starts with '[' character or spaces before '['.
        /// Returns null and does not advance if no matching ] is found.
        /// Note the precedence:  code backticks have precedence over label bracket
        /// markers, which have precedence over *, _, and other inline formatting
        /// markers. So, 2 below contains a link while 1 does not:
        /// 1. [a link `with a ](/url)` character
        /// 2. [a link *with emphasized ](/url) text*        /// </summary>
        public static StringPart? ParseReferenceLabel(Subject subj)
        {
            var startPos = subj.Position;
            var source = subj.Buffer;
            var len = subj.Length;

            while (subj.Position < len)
            {
                var c = subj.Buffer[subj.Position];
                if (c == ' ' || c == '\n')
                {
                    subj.Position++;
                    continue;
                }
                else if (c == '[')
                {
                    subj.Position++;
                    break;
                }
                else
                {
                    subj.Position = startPos;
                    return null;
                }
            }

            var labelStartPos = subj.Position;

            len = subj.Position + Reference.MaximumReferenceLabelLength;
            if (len > source.Length)
                len = source.Length;

            subj.Position = source.IndexOfAny(BracketSpecialCharacters, subj.Position, len - subj.Position);
            while (subj.Position > -1)
            {
                var c = source[subj.Position];
                if (c == '\\')
                {
                    subj.Position += 2;
                    if (subj.Position >= len)
                        break;

                    subj.Position = source.IndexOfAny(BracketSpecialCharacters, subj.Position, len - subj.Position);
                }
                else if (c == '[')
                {
                    break;
                }
                else
                {
                    var label = new StringPart(source, labelStartPos, subj.Position - labelStartPos);
                    subj.Position++;
                    return label;
                }
            }

            subj.Position = startPos;
            return null;
        }

        // Parse reference.  Assumes string begins with '[' character.
        // Modify refmap if a reference is encountered.
        // Return 0 if no reference found, otherwise position of subject
        // after reference is parsed.
        public static int ParseReference(Subject subj, InlineParserParameters parameters)
        {
            string title;
            var startPos = subj.Position;

            // parse label:
            var lab = ParseReferenceLabel(subj);
            if (lab == null || lab.Value.Length > Reference.MaximumReferenceLabelLength)
                goto INVALID;

            if (!HasNonWhitespace(lab.Value))
                goto INVALID;

            // colon:
            if (peek_char(subj) == ':')
                subj.Position++;
            else
                goto INVALID;

            // parse link url:
            spnl(subj);
            var matchlen = Scanner.scan_link_url(subj.Buffer, subj.Position, subj.Length);
            if (matchlen == 0)
                goto INVALID;

            var url = subj.Buffer.Substring(subj.Position, matchlen);
            url = CleanUrl(url);
            subj.Position += matchlen;

            // parse optional link_title
            var beforetitle = subj.Position;
            spnl(subj);

            matchlen = Scanner.scan_link_title(subj.Buffer, subj.Position, subj.Length);
            if (matchlen > 0)
            {
                title = subj.Buffer.Substring(subj.Position, matchlen);
                title = CleanTitle(title);
                subj.Position += matchlen;
            }
            else
            {
                subj.Position = beforetitle;
                title = string.Empty;
            }

            char c;
            // parse final spaces and newline:
            while ((c = peek_char(subj)) == ' ') subj.Position++;

            if (c == '\n')
            {
                subj.Position++;
            }
            else if (c != '\0')
            {
                if (matchlen > 0)
                { // try rewinding before title
                    subj.Position = beforetitle;
                    while ((c = peek_char(subj)) == ' ') subj.Position++;
                    if (c == '\n')
                        subj.Position++;
                    else if (c != '\0')
                       goto INVALID;
                }
                else
                {
                    goto INVALID;
                }
            }

            // insert reference into refmap
            AddReference(subj.ReferenceMap, lab.Value, url, title, parameters);

            return subj.Position;

        INVALID:
            subj.Position = startPos;
            return 0;
        }

        /// <summary>
        /// Determines if the given string has non-whitespace characters in it.
        /// </summary>
        private static bool HasNonWhitespace(Syntax.StringPart part)
        {
            var s = part.Source;
            var i = part.StartIndex;
            var l = i + part.Length;

            while (i < l)
            {
                if (!Utilities.IsWhitespace(s[i]))
                    return true;

                i++;
            }

            return false;
        }
    }
}
