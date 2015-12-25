using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal static class HtmlFormatter
    {
        private static readonly char[] EscapeHtmlCharacters = { '&', '<', '>', '"' };
        private const string HexCharacters = "0123456789ABCDEF";

        private static readonly char[] EscapeHtmlLessThan = "&lt;".ToCharArray();
        private static readonly char[] EscapeHtmlGreaterThan = "&gt;".ToCharArray();
        private static readonly char[] EscapeHtmlAmpersand = "&amp;".ToCharArray();
        private static readonly char[] EscapeHtmlQuote = "&quot;".ToCharArray();

        private static readonly bool[] UrlSafeCharacters = {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, true,  false, true,  true,  true,  false, false, true,  true,  true,  true,  true,  true,  true,  true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, true,  false, true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, true, 
            false, true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, false
        };

        /// <summary>
        /// Escapes special URL characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        internal static void EscapeUrl(string input, HtmlTextWriter target)
        {
            if (input == null)
                return;

            char c;
            int lastPos = 0;
            int len = input.Length;
            char[] buffer;

            if (target.Buffer.Length < len)
                buffer = target.Buffer = input.ToCharArray();
            else
            {
                buffer = target.Buffer;
                input.CopyTo(0, buffer, 0, len);
            }

            // since both \r and \n are not url-safe characters and will be encoded, all calls are
            // made to WriteConstant.
            for (var pos = 0; pos < len; pos++)
            {
                c = buffer[pos];

                if (c == '&')
                {
                    target.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;
                    target.WriteConstant(EscapeHtmlAmpersand);
                }
                else if (c < 128 && !UrlSafeCharacters[c])
                {
                    target.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    target.WriteConstant(new[] { '%', HexCharacters[c / 16], HexCharacters[c % 16] });
                }
                else if (c > 127)
                {
                    target.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && len != lastPos)
                    {
                        // this char is the first of UTF-32 character pair
                        bytes = Encoding.UTF8.GetBytes(new[] { c, buffer[lastPos] });
                        lastPos = ++pos + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c });
                    }

                    for (var i = 0; i < bytes.Length; i++)
                        target.WriteConstant(new[] { '%', HexCharacters[bytes[i] / 16], HexCharacters[bytes[i] % 16] });
                }
            }

            target.WriteConstant(buffer, lastPos, len - lastPos);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        internal static void EscapeHtml(StringPart input, HtmlTextWriter target)
        {
            if (input.Length == 0)
                return;

            int pos;
            int lastPos = input.StartIndex;
            char[] buffer;

            if (target.Buffer.Length < input.Length)
                buffer = target.Buffer = new char[input.Length];
            else
                buffer = target.Buffer;
                 
            input.Source.CopyTo(input.StartIndex, buffer, 0, input.Length);

            while ((pos = input.Source.IndexOfAny(EscapeHtmlCharacters, lastPos, input.Length - lastPos + input.StartIndex)) != -1)
            {
                target.Write(buffer, lastPos - input.StartIndex, pos - lastPos);
                lastPos = pos + 1;

                switch (input.Source[pos])
                {
                    case '<':
                        target.WriteConstant(EscapeHtmlLessThan);
                        break;
                    case '>':
                        target.WriteConstant(EscapeHtmlGreaterThan);
                        break;
                    case '&':
                        target.WriteConstant(EscapeHtmlAmpersand);
                        break;
                    case '"':
                        target.WriteConstant(EscapeHtmlQuote);
                        break;
                }
            }

            target.Write(buffer, lastPos - input.StartIndex, input.Length - lastPos + input.StartIndex);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        internal static void EscapeHtml(StringContent inp, HtmlTextWriter target)
        {
            int pos;
            int lastPos;
            char[] buffer = target.Buffer;

            var parts = inp.RetrieveParts();
            for (var i = parts.Offset; i < parts.Offset + parts.Count; i++)
            {
                var part = parts.Array[i];

                if (buffer.Length < part.Length)
                    buffer = target.Buffer = new char[part.Length];

                part.Source.CopyTo(part.StartIndex, buffer, 0, part.Length);

                lastPos = part.StartIndex;
                while ((pos = part.Source.IndexOfAny(EscapeHtmlCharacters, lastPos, part.Length - lastPos + part.StartIndex)) != -1)
                {
                    target.Write(buffer, lastPos - part.StartIndex, pos - lastPos);
                    lastPos = pos + 1;

                    switch (part.Source[pos])
                    {
                        case '<':
                            target.WriteConstant(EscapeHtmlLessThan);
                            break;
                        case '>':
                            target.WriteConstant(EscapeHtmlGreaterThan);
                            break;
                        case '&':
                            target.WriteConstant(EscapeHtmlAmpersand);
                            break;
                        case '"':
                            target.WriteConstant(EscapeHtmlQuote);
                            break;
                    }
                }

                target.Write(buffer, lastPos - part.StartIndex, part.Length - lastPos + part.StartIndex);
            }
        }
        
        public static void BlocksToHtml(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            var wrapper = new HtmlTextWriter(writer);
            BlocksToHtmlInner(wrapper, block, settings);
        }

        internal static void PrintPosition(HtmlTextWriter writer, Block block)
        {
            writer.WriteConstant(" data-sourcepos=\"");
            writer.WriteConstant(block.SourcePosition.ToString(CultureInfo.InvariantCulture));
            writer.Write('-');
            writer.WriteConstant(block.SourceLastPosition.ToString(CultureInfo.InvariantCulture));
            writer.WriteConstant("\"");
        }

        internal static void PrintPosition(HtmlTextWriter writer, Inline inline)
        {
            writer.WriteConstant(" data-sourcepos=\"");
            writer.WriteConstant(inline.SourcePosition.ToString(CultureInfo.InvariantCulture));
            writer.Write('-');
            writer.WriteConstant(inline.SourceLastPosition.ToString(CultureInfo.InvariantCulture));
            writer.WriteConstant("\"");
        }

        private static void BlocksToHtmlInner(HtmlTextWriter writer, Block block, CommonMarkSettings settings)
        {
            var stack = new Stack<BlockStackEntry>();
            var inlineStack = new Stack<InlineStackEntry>();
            bool visitChildren;
            string stackLiteral = null;
            bool stackTight = false;
            bool tight = false;
            bool trackPositions = settings.TrackSourcePosition;

            var parameters = settings.FormatterParameters;
            IBlockFormatter[] formatters = parameters.BlockFormatters;
            IBlockFormatter formatter;
            bool? isRenderPlainTextInlines;
            bool? isStackTight;

            while (block != null)
            {
                visitChildren = false;

                formatter = formatters[(int)block.Tag];
                if (formatter == null)
                {
                    throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                visitChildren = formatter.WriteOpening(writer, block, tight);
                stackLiteral = formatter.GetClosing(parameters.HtmlFormatter, block, tight);

                isRenderPlainTextInlines = formatter.IsRenderPlainTextInlines(block);
                if (isRenderPlainTextInlines == false)
                {
                    InlinesToHtml(writer, block.InlineContent, settings, inlineStack);
                    if (block.Tag == BlockTag.Paragraph)
                        writer.WriteConstant(stackLiteral);
                    else
                        writer.WriteLineConstant(stackLiteral);
                    stackLiteral = null;
                }

                isStackTight = formatter.IsStackTight(block, tight);
                if (isStackTight.HasValue)
                    stackTight = isStackTight.Value;

                if (visitChildren)
                {
                    stack.Push(new BlockStackEntry(stackLiteral, block.NextSibling, tight));

                    tight = stackTight;
                    block = block.FirstChild;
                }
                else if (block.NextSibling != null)
                {
                    block = block.NextSibling;
                }
                else
                {
                    block = null;
                }

                while (block == null && stack.Count > 0)
                {
                    var entry = stack.Pop();

                    writer.WriteLineConstant(entry.Literal);
                    tight = entry.IsTight;
                    block = entry.Target;
                }
            }
        }

        /// <summary>
        /// Writes the inline list to the given writer as plain text (without any HTML tags).
        /// </summary>
        /// <seealso href="https://github.com/jgm/CommonMark/issues/145"/>
        private static void InlinesToPlainText(HtmlTextWriter writer, Inline inline, Stack<InlineStackEntry> stack)
        {
            bool withinLink = false;
            bool stackWithinLink = false; 
            bool visitChildren;
            string stackLiteral = null;
            var origStackCount = stack.Count;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                    case InlineTag.Code:
                    case InlineTag.RawHtml:
                        EscapeHtml(inline.LiteralContentValue, writer);
                        break;

                    case InlineTag.LineBreak:
                    case InlineTag.SoftBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.Link:
                        if (withinLink)
                        {
                            writer.Write('[');
                            stackLiteral = "]";
                            visitChildren = true;
                            stackWithinLink = true;
                        }
                        else
                        {
                            visitChildren = true;
                            stackWithinLink = true;
                            stackLiteral = string.Empty;
                        }
                        break;

                    case InlineTag.Image:
                        visitChildren = true;
                        stackWithinLink = true;
                        stackLiteral = string.Empty;
                        break;

                    case InlineTag.Strong:
                    case InlineTag.Emphasis:
                    case InlineTag.Strikethrough:
                        stackLiteral = string.Empty;
                        stackWithinLink = withinLink;
                        visitChildren = true;
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                if (visitChildren)
                {
                    stack.Push(new InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

                    withinLink = stackWithinLink;
                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else
                {
                    inline = null;
                }

                while (inline == null && stack.Count > origStackCount)
                {
                    var entry = stack.Pop();
                    writer.WriteConstant(entry.Literal);
                    inline = entry.Target;
                    withinLink = entry.IsWithinLink;
                }
            }
        }

        /// <summary>
        /// Writes the inline list to the given writer as HTML code. 
        /// </summary>
        private static void InlinesToHtml(HtmlTextWriter writer, Inline inline, CommonMarkSettings settings, Stack<InlineStackEntry> stack)
        {
            var uriResolver = settings.UriResolver;
            bool withinLink = false;
            bool stackWithinLink = false;
            bool visitChildren;
            bool trackPositions = settings.TrackSourcePosition;
            string stackLiteral = null;

            var parameters = settings.FormatterParameters;
            IInlineFormatter[] formatters = parameters.InlineFormatters;
            IInlineFormatter formatter;
            bool? isRenderPlainTextInlines;

            while (inline != null)
            {
                visitChildren = false;

                formatter = formatters[(int)inline.Tag];
                if (formatter == null)
                {
                    throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                visitChildren = formatter.WriteOpening(writer, inline, withinLink);
                stackLiteral = formatter.GetClosing(parameters.HtmlFormatter, inline, withinLink);

                isRenderPlainTextInlines = formatter.IsRenderPlainTextInlines(inline);
                if (isRenderPlainTextInlines == true)
                {
                    InlinesToPlainText(writer, inline.FirstChild, stack);
                    writer.WriteConstant(stackLiteral);
                    stackLiteral = null;
                }
                else if (isRenderPlainTextInlines == false)
                {
                    EscapeHtml(inline.LiteralContentValue, writer);
                }

                stackWithinLink = formatter.IsStackWithinLink(inline, withinLink);

                if (visitChildren)
                {
                    stack.Push(new InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

                    withinLink = stackWithinLink;
                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else
                {
                    inline = null;
                }

                while (inline == null && stack.Count > 0)
                {
                    var entry = stack.Pop();
                    writer.WriteConstant(entry.Literal);
                    inline = entry.Target;
                    withinLink = entry.IsWithinLink;
                }
            }
        }

        private struct BlockStackEntry
        {
            public readonly string Literal;
            public readonly Block Target;
            public readonly bool IsTight;
            public BlockStackEntry(string literal, Block target, bool isTight)
            {
                Literal = literal;
                Target = target;
                IsTight = isTight;
            }
        }
        private struct InlineStackEntry
        {
            public readonly string Literal;
            public readonly Inline Target;
            public readonly bool IsWithinLink;
            public InlineStackEntry(string literal, Inline target, bool isWithinLink)
            {
                Literal = literal;
                Target = target;
                IsWithinLink = isWithinLink;
            }
        }
    }

    internal sealed class HtmlFormatterImpl : IHtmlFormatter
    {
        internal HtmlFormatterImpl()
        {
        }

        string IHtmlFormatter.EscapeHtml(StringPart part)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    var htmlWriter = new HtmlTextWriter(writer);
                    HtmlFormatter.EscapeHtml(part, htmlWriter);
                }
                var buffer = stream.ToArray();
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
        }

        string IHtmlFormatter.EscapeUrl(string url)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    var htmlWriter = new HtmlTextWriter(writer);
                    HtmlFormatter.EscapeUrl(url, htmlWriter);
                }
                var buffer = stream.ToArray();
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
        }

        string IHtmlFormatter.PrintPosition(Inline element)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    var htmlWriter = new HtmlTextWriter(writer);
                    HtmlFormatter.PrintPosition(htmlWriter, element);
                }
                var buffer = stream.ToArray();
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
        }
    }
}
