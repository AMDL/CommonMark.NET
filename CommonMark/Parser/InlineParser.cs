using CommonMark.Syntax;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    /// <summary>
    /// Base inline element parser class.
    /// </summary>
    public abstract class InlineParser : ElementParser, IInlineParser
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="c">Handled character.</param>
        public InlineParser(CommonMarkSettings settings, char c)
        {
            this.Settings = settings;
            this.Character = c;
        }

        #endregion Constructor

        /// <summary>
        /// Gets the opening character handled by this parser.
        /// </summary>
        /// <value>Character that can open a handled element.</value>
        public char Character
        {
            get;
        }

        /// <summary>
        /// Handles an element.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        public abstract Inline Handle(Block container, Subject subj);

        /// <summary>
        /// Gets the common settings object.
        /// </summary>
        protected CommonMarkSettings Settings { get; }

        /// <summary>
        /// Creates a new <see cref="Inline"/> element that represents string content but the given content
        /// is processed to decode any HTML entities in it.
        /// This method is guaranteed to return just one Inline, without nested elements.
        /// </summary>
        protected static Inline ParseStringEntities(string s)
        {
            string result = null;
            StringBuilder builder = null;

            int searchpos;
            char c;
            var subj = new Subject(s, null);

            while ('\0' != (c = InlineMethods.peek_char(subj)))
            {
                if (result != null)
                {
                    if (builder == null)
                        builder = new StringBuilder(result, s.Length);
                    else
                        builder.Append(result);
                }

                if (c == '&')
                {
                    result = ParseEntity(subj);
                }
                else
                {
                    searchpos = subj.Buffer.IndexOf('&', subj.Position);
                    if (searchpos == -1)
                        searchpos = subj.Length;

                    result = subj.Buffer.Substring(subj.Position, searchpos - subj.Position);
                    subj.Position = searchpos;
                }
            }

            if (builder == null)
                return new Inline(result);

            builder.Append(result);

            return new Inline(builder.ToString());
        }

        /// <summary>
        /// Parses the entity at the current position.
        /// Assumes that there is a <c>&amp;</c> at the current position.
        /// </summary>
        protected static string ParseEntity(Subject subj)
        {
            int match;
            string entity;
            int numericEntity;
            var origPos = subj.Position;
            match = Scanner.scan_entity(subj.Buffer, subj.Position, subj.Length - subj.Position, out entity, out numericEntity);
            if (match > 0)
            {
                subj.Position += match;

                if (entity != null)
                {
                    entity = EntityDecoder.DecodeEntity(entity);
                    if (entity != null)
                        return entity;

                    return subj.Buffer.Substring(origPos, match);
                }
                else if (numericEntity > 0)
                {
                    entity = EntityDecoder.DecodeEntity(numericEntity);
                    if (entity != null)
                        return entity;
                }

                return "\uFFFD";
            }
            else
            {
                subj.Position++;
                return "&";
            }
        }

        /// <summary>
        /// Parses a left bracket opener.
        /// </summary>
        /// <remarks>Original: Inline HandleLeftSquareBracket(Subject subj, bool isImage)</remarks>
        protected static Inline HandleLeftBracket(Subject subj, string content, InlineStack.InlineStackFlags flags, int position)
        {
            var inlText = new Inline(content, position, subj.Position + 1);

            // move past the '['
            subj.Position++;

            return AppendStackEntry(subj, inlText, InlineStack.InlineStackPriority.Links, flags, '[');
        }

        /// <summary>
        /// Appends an element to the stack.
        /// </summary>
        protected static Inline AppendStackEntry(Subject subj, Inline inlText, InlineStack.InlineStackPriority priority, InlineStack.InlineStackFlags flags, char delimiter, int numdelims = 0)
        {
            var istack = new InlineStack
            {
                DelimiterCount = numdelims,
                Delimiter = delimiter,
                StartingInline = inlText,
                StartPosition = subj.Position,
                Priority = priority,
                Flags = flags,
            };

            InlineStack.AppendStackEntry(istack, subj);

            return inlText;
        }

        /// <summary>
        /// Creates an inline link element and adjusts the subject and label positions.
        /// </summary>
        /// <returns>Inline element.</returns>
        protected static Inline CreateLink(Subject subj, Inline label, string url, string title, int matchlen)
        {
            var result = new Inline
            {
                Tag = InlineTag.Link,
                FirstChild = label,
                TargetUrl = url,
                LiteralContent = title,
                SourcePosition = subj.Position - 1,
                SourceLastPosition = subj.Position + matchlen,
            };

            label.SourcePosition = subj.Position;
            label.SourceLastPosition = subj.Position + matchlen - 1;

            subj.Position += matchlen;

            return result;
        }

        /// <summary>
        /// Parses an inline element from the subject. The subject position is updated to after the element.
        /// </summary>
        private static Inline ParseInline(Block container, Subject subj, InlineParserParameters parameters)
        {
            var handlers = parameters.Handlers;
            var specialCharacters = parameters.SpecialCharacters;

            var c = subj.Buffer[subj.Position];

            var handler = c < handlers.Length ? handlers[c] : null;

            if (handler != null)
                return handler(container, subj);

            var startpos = subj.Position;

            // we read until we hit a special character
            // +1 is so that any special character at the current position is ignored.
            var endpos = subj.Buffer.IndexOfAny(specialCharacters, startpos + 1, subj.Length - startpos - 1);

            if (endpos == -1)
                endpos = subj.Length;

            subj.Position = endpos;

            // if we're at a newline, strip trailing spaces.
            if (endpos < subj.Length && subj.Buffer[endpos] == '\n')
                while (endpos > startpos && subj.Buffer[endpos - 1] == ' ')
                    endpos--;

            return new Inline(subj.Buffer, startpos, endpos - startpos, startpos, endpos, c);
        }

        /// <summary>
        /// Parses the inline contents of a block element.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <param name="parameters">Inline parser parameters.</param>
        /// <returns>The first inline or <c>null</c>.</returns>
        /// <remarks>Original: Inline parse_inlines(Subject subj, Dictionary&lt;string, Reference&gt; refmap)</remarks>
        internal static Inline ParseInlines(Block container, Subject subj, InlineParserParameters parameters)
        {
            var len = subj.Length;

            if (len == 0)
                return null;

            var first = ParseInline(container, subj, parameters);
            subj.LastInline = first.LastSibling;

            Inline cur;
            while (subj.Position < len)
            {
                cur = ParseInline(container, subj, parameters);
                if (cur != null)
                {
                    subj.LastInline.NextSibling = cur;
                    subj.LastInline = cur.LastSibling;
                }
            }

            InlineStack.PostProcessInlineStack(subj, subj.FirstPendingInline, subj.LastPendingInline, InlineStack.InlineStackPriority.Maximum, parameters);

            return first;
        }

        #region InitializeParsers

        internal static IEnumerable<IInlineParser> InitializeParsers(InlineParserParameters parameters)
        {
            var parsers = new List<IInlineParser>(InitializeBaseParsers(parameters.Settings));
            parsers.AddRange(InitializeDelimiterParsers(parameters));
            return parsers;
        }

        private static IEnumerable<IInlineParser> InitializeBaseParsers(CommonMarkSettings settings)
        {
            yield return new Inlines.LinkOpenerParser(settings);
            yield return new Inlines.LinkCloserParser(settings);
            yield return new Inlines.ImageLinkParser(settings);
            yield return new Inlines.LineBreakParser(settings);
            yield return new Inlines.CodeParser(settings);
            yield return new Inlines.EscapeParser(settings);
            yield return new Inlines.EntityParser(settings);
            yield return new Inlines.AutoUriParser(settings);
            yield return new Inlines.AutoEmailParser(settings);
            yield return new Inlines.HtmlTagParser(settings);
            yield return new Inlines.LessThanParser(settings);
        }

        private static IEnumerable<IInlineParser> InitializeDelimiterParsers(InlineParserParameters parameters)
        {
            var delimChars = parameters.DelimiterCharacters;
            for (var i = 0; i < delimChars.Length; i++)
            {
                var delimiters = delimChars[i];
                if (!delimiters.IsEmpty)
                    yield return new Inlines.DelimiterParser(delimiters, parameters, (char)i);
            }
        }

        #endregion InitializeParsers

        #region InitializeDelimiterHandlers

        internal static IEnumerable<IInlineDelimiterHandler> InitializeDelimiterHandlers()
        {
            yield return new Inlines.Delimiters.AsteriskHandler();
            yield return new Inlines.Delimiters.UnderscoreHandler();
        }

        #endregion InitializeDelimiterHandlers
    }
}
