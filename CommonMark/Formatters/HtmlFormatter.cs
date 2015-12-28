using System.Collections.Generic;
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
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal static void EscapeHtml(StringPart input, HtmlTextWriter target)
        {
            if (input.Length == 0)
                return;

            EscapeHtmlInner(input, target);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        internal static void EscapeHtml(StringContent inp, HtmlTextWriter target)
        {
            var parts = inp.RetrieveParts();
            for (var i = parts.Offset; i < parts.Offset + parts.Count; i++)
            {
                EscapeHtmlInner(parts.Array[i], target);
            }
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static void EscapeHtmlInner(StringPart part, HtmlTextWriter target)
        {
            if (target.Buffer.Length < part.Length)
                target.Buffer = new char[part.Length];

            char[] buffer = target.Buffer;

            part.Source.CopyTo(part.StartIndex, buffer, 0, part.Length);

            int lastPos = part.StartIndex;
            int pos;

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

        public static void BlocksToHtml(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            var blockFormatter = new BlockHtmlFormatter(settings);
            var inlineFormatter = new InlineHtmlFormatter(settings);
            var wrapper = new HtmlTextWriter(writer);
            blockFormatter.Format(wrapper, inlineFormatter, block);
        }
    }

    internal abstract class ElementHtmlFormatter<TElement, TTag, TFormatter, TStackEntry>
        where TFormatter : IElementFormatter<TElement, TTag>
    {
        protected delegate bool WriteOpeningDelegate(IHtmlTextWriter writer, TElement element);
        protected delegate string GetClosingDelegate(TElement element);
        protected delegate bool IsRenderDelegate(TElement element);

        protected readonly Stack<TStackEntry> stack = new Stack<TStackEntry>();

        private readonly WriteOpeningDelegate[] writeOpening;
        private readonly GetClosingDelegate[] getClosing;
        private readonly IsRenderDelegate[] isHtmlInlines;

        protected ElementHtmlFormatter(FormatterParameters parameters, TFormatter[] formatters, int count)
        {
            Parameters = parameters;
            Formatters = formatters;
            Count = count;

            writeOpening = new WriteOpeningDelegate[count];
            getClosing = new GetClosingDelegate[count];
            isHtmlInlines = new IsRenderDelegate[count];

            for (int i = 0; i < count; i++)
            {
                writeOpening[i] = formatters[i].WriteOpening;
                getClosing[i] = formatters[i].GetClosing;
                isHtmlInlines[i] = formatters[i].IsHtmlInlines;
            }
        }

        protected FormatterParameters Parameters
        {
            get;
        }

        protected TFormatter[] Formatters
        {
            get;
        }

        protected int Count
        {
            get;
        }

        protected bool TrackPositions
        {
            get { return Parameters.TrackPositions; }
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        protected bool WriteOpening(int index, IHtmlTextWriter writer, TElement element)
        {
            return writeOpening[index](writer, element);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        protected string GetClosing(int index, TElement element)
        {
            return getClosing[index](element);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        protected bool IsHtmlInlines(int index, TElement element)
        {
            return isHtmlInlines[index](element);
        }
    }

    internal sealed class BlockHtmlFormatter : ElementHtmlFormatter<Block, BlockTag, IBlockFormatter, BlockHtmlFormatter.Entry>
    {
        internal struct Entry
        {
            public string Literal { get; }
            public Block Target { get; }
            public bool IsTight { get; }

            public Entry(string literal, Block target, bool isTight)
            {
                Literal = literal;
                Target = target;
                IsTight = isTight;
            }
        }

        private delegate bool IsTightDelegate(Block element);

        private readonly long isList;
        private readonly long isListItem;
        private readonly IsTightDelegate[] isTight;

        private bool? IsForceTightLists
        {
            get { return Parameters.IsForceTightLists; }
        }

        public BlockHtmlFormatter(CommonMarkSettings settings)
            : base(settings.FormatterParameters, settings.FormatterParameters.BlockFormatters, (int)BlockTag.Count)
        {
            isTight = new IsTightDelegate[Count];

            long m = 1;
            for (var i = 0; i < Count; i++)
            {
                if (Formatters[i].IsList)
                    isList |= m;
                if (Formatters[i].IsListItem)
                    isListItem |= m;
                isTight[i] = Formatters[i].IsTight;
                m <<= 1;
            }
        }

        public void Format(HtmlTextWriter writer, InlineHtmlFormatter inlineFormatter, Block block)
        {
            bool visitChildren;
            string stackLiteral;
            bool renderHtmlInlines;
            bool tight = false;
            int index;

            while (block != null)
            {
                index = (int)block.Tag;

                if (IsParagraph(index))
                {
                    if (!tight)
                    {
                        writer.WriteConstant("<p");
                        if (TrackPositions)
                            writer.WritePosition(block);
                        writer.Write('>');
                    }
                    visitChildren = true;
                    renderHtmlInlines = true;
                    inlineFormatter.Format(writer, block.InlineContent);
                    if (!tight)
                    {
                        writer.WriteConstant("</p>");
                    }
                }
                else
                {
                    visitChildren = WriteOpening(index, writer, block);
                    renderHtmlInlines = IsHtmlInlines(index, block);
                    if (renderHtmlInlines)
                    {
                        inlineFormatter.Format(writer, block.InlineContent);
                        stackLiteral = GetClosing(index, block);
                        writer.WriteLineConstant(stackLiteral);
                    }
                }

                if (visitChildren)
                {
                    stackLiteral = !renderHtmlInlines
                        ? GetClosing(index, block)
                        : null;

                    stack.Push(new Entry(stackLiteral, block.NextSibling, tight));

                    tight = IsTight(index, block, tight);
                    block = block.FirstChild;
                }
                else
                {
                    block = block.NextSibling;
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

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsTight(int index, Block element, bool tight)
        {
            if (IsList(index))
                return isTight[index](element);
            if (!IsListItem(index))
                return false;
            if (IsForceTightLists.HasValue)
                return IsForceTightLists.Value;
            return tight;
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsList(int index)
        {
            return 0 != (isList & (1 << index));
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsListItem(int index)
        {
            return 0 != (isListItem & (1 << index));
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsParagraph(int index)
        {
            return index == (int)BlockTag.Paragraph;
        }
    }

    internal sealed class InlineHtmlFormatter : ElementHtmlFormatter<Inline, InlineTag, IInlineFormatter, InlineHtmlFormatter.Entry>
    {
        internal struct Entry
        {
            public string Literal { get; }
            public Inline Target { get; }
            public bool? IsPlaintext { get; }
            public bool IsWithinLink { get; }

            public Entry(string literal, Inline target, bool? isPlaintext, bool isWithinLink)
            {
                Literal = literal;
                Target = target;
                IsPlaintext = isPlaintext;
                IsWithinLink = isWithinLink;
            }
        }

        private delegate string GetInfixDelegate(Inline element);

        private readonly WriteOpeningDelegate[] writePlaintextOpening;
        private readonly GetClosingDelegate[] getPlaintextClosing;
        private readonly GetInfixDelegate[] getInfix;
        private readonly IsRenderDelegate[] isPlaintextInlines;

        public InlineHtmlFormatter(CommonMarkSettings settings)
            : base(settings.FormatterParameters, settings.FormatterParameters.InlineFormatters, (int)InlineTag.Count)
        {
            writePlaintextOpening = new WriteOpeningDelegate[Count];
            getPlaintextClosing = new GetClosingDelegate[Count];
            getInfix = new GetInfixDelegate[Count];
            isPlaintextInlines = new IsRenderDelegate[Count];

            for (var i = 0; i < Count; i++)
            {
                writePlaintextOpening[i] = Formatters[i].WritePlaintextOpening;
                getPlaintextClosing[i] = Formatters[i].GetPlaintextClosing;
                getInfix[i] = Formatters[i].GetInfix;
                isPlaintextInlines[i] = Formatters[i].IsPlaintextInlines;
            }
        }

        /// <summary>
        /// Writes the inline list to the given writer as HTML code. 
        /// </summary>
        public void Format(HtmlTextWriter writer, Inline inline)
        {
            var stackPlaintext = false;
            bool? plaintext = false;
            bool withinLink = false;
            bool isLink;
            bool visitChildren;
            string stackLiteral;
            bool renderHtmlInlines = false;
            int index;

            while (inline != null)
            {
                index = (int)inline.Tag;
                isLink = IsLink(index);

                switch (plaintext)
                {
                    case false:
                        if (isLink && withinLink)
                        {
                            writer.Write('[');
                            stackLiteral = "]";
                            visitChildren = true;
                        }
                        else
                        {
                            visitChildren = WriteOpening(index, writer, inline);
                            stackLiteral = GetClosing(index, inline);
                            stackPlaintext = IsPlaintextInlines(index, inline);
                            renderHtmlInlines = IsHtmlInlines(index, inline);
                            if (stackPlaintext)
                            {
                                plaintext = true;
                                visitChildren = true;
                            }
                            else if (renderHtmlInlines)
                            {
                                HtmlFormatter.EscapeHtml(inline.LiteralContentValue, writer);
                            }
                        }
                        break;

                    case true:
                        if (isLink && withinLink)
                        {
                            writer.Write('[');
                            stackLiteral = "]";
                            visitChildren = true;
                        }
                        else
                        {
                            visitChildren = WritePlaintextOpening(index, writer, inline);
                            stackLiteral = GetPlaintextClosing(index, inline);
                        }
                        break;

                    default:
                        visitChildren = false;
                        stackLiteral = null;
                        HtmlFormatter.EscapeHtml(inline.LiteralContentValue, writer);
                        break;
                }

                if (visitChildren)
                {
                    stack.Push(new Entry(stackLiteral, inline.NextSibling, plaintext, withinLink));

                    if (stackPlaintext && renderHtmlInlines && inline.LiteralContentValue.Length > 0)
                    {
                        stackLiteral = GetInfix(index, inline);
                        stack.Push(new Entry(stackLiteral, inline, null, withinLink));
                    }

                    withinLink |= isLink;
                    inline = inline.FirstChild;
                }
                else
                {
                    inline = inline.NextSibling;
                }

                while (inline == null && stack.Count > 0)
                {
                    var entry = stack.Pop();
                    writer.WriteConstant(entry.Literal);
                    inline = entry.Target;
                    withinLink = entry.IsWithinLink;
                    plaintext = entry.IsPlaintext;
                }
            }
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool WritePlaintextOpening(int index, IHtmlTextWriter writer, Inline element)
        {
            return writePlaintextOpening[index](writer, element);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private string GetInfix(int index, Inline element)
        {
            return getInfix[index](element);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private string GetPlaintextClosing(int index, Inline element)
        {
            return getPlaintextClosing[index](element);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsPlaintextInlines(int index, Inline element)
        {
            return isPlaintextInlines[index](element);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsLink(int index)
        {
            return index == (int)InlineTag.Link;
        }
    }
}
