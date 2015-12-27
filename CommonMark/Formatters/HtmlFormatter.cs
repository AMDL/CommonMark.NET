using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal class HtmlFormatter
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

        private delegate bool WriteOpeningDelegate<T>(IHtmlTextWriter writer, T element, bool flag);

        private delegate string GetInfixDelegate<T>(T element);

        private delegate string GetClosingDelegate<T>(T element, bool flag);

        private delegate bool RenderDelegate<T>(T element);

        private delegate bool InheritDelegate<T>(T element, bool flag);

        private readonly CommonMarkSettings settings;
        private readonly Stack<BlockStackEntry> blockStack;
        private readonly Stack<InlineStackEntry> inlineStack;

        private readonly WriteOpeningDelegate<Block>[] blockOpeners;
        private readonly GetClosingDelegate<Block>[] blockClosers;
        private readonly RenderDelegate<Block>[] blockHtmlRenderers;
        private readonly InheritDelegate<Block>[] blockStackers;

        private readonly WriteOpeningDelegate<Inline>[] inlineOpeners;
        private readonly GetInfixDelegate<Inline>[] inlineInfixers;
        private readonly GetClosingDelegate<Inline>[] inlineClosers;
        private readonly RenderDelegate<Inline>[] inlineHtmlRenderers;
        private readonly RenderDelegate<Inline>[] inlinePlaintextRenderers;
        private readonly WriteOpeningDelegate<Inline>[] inlinePlaintextOpeners;
        private readonly GetClosingDelegate<Inline>[] inlinePlaintextClosers;
        private readonly InheritDelegate<Inline>[] inlineStackers;

        public HtmlFormatter(CommonMarkSettings settings)
        {
            this.settings = settings;

            blockStack = new Stack<BlockStackEntry>();
            inlineStack = new Stack<InlineStackEntry>();

            var blockFormatters = settings.FormatterParameters.BlockFormatters;

            blockOpeners = new WriteOpeningDelegate<Block>[(int)BlockTag.Count];
            blockClosers = new GetClosingDelegate<Block>[(int)BlockTag.Count];
            blockHtmlRenderers = new RenderDelegate<Block>[(int)BlockTag.Count];
            blockStackers = new InheritDelegate<Block>[(int)BlockTag.Count];

            for (var i = 0; i < (int)BlockTag.Count; i++)
            {
                blockOpeners[i] = blockFormatters[i].WriteOpening;
                blockClosers[i] = blockFormatters[i].GetClosing;
                blockHtmlRenderers[i] = blockFormatters[i].IsHtmlInlines;
                blockStackers[i] = blockFormatters[i].IsTight;
            }

            var inlineFormatters = settings.FormatterParameters.InlineFormatters;

            inlineOpeners = new WriteOpeningDelegate<Inline>[(int)InlineTag.Count];
            inlineInfixers = new GetInfixDelegate<Inline>[(int)InlineTag.Count];
            inlineClosers = new GetClosingDelegate<Inline>[(int)InlineTag.Count];
            inlineHtmlRenderers = new RenderDelegate<Inline>[(int)InlineTag.Count];
            inlinePlaintextRenderers = new RenderDelegate<Inline>[(int)InlineTag.Count];
            inlinePlaintextOpeners = new WriteOpeningDelegate<Inline>[(int)InlineTag.Count];
            inlinePlaintextClosers = new GetClosingDelegate<Inline>[(int)InlineTag.Count];
            inlineStackers = new InheritDelegate<Inline>[(int)InlineTag.Count];

            for (var i = 0; i < (int)InlineTag.Count; i++)
            {
                inlineOpeners[i] = inlineFormatters[i].WriteOpening;
                inlineInfixers[i] = inlineFormatters[i].GetInfix;
                inlineClosers[i] = inlineFormatters[i].GetClosing;
                inlineHtmlRenderers[i] = inlineFormatters[i].IsHtmlInlines;
                inlinePlaintextRenderers[i] = inlineFormatters[i].IsPlaintextInlines;
                inlinePlaintextOpeners[i] = inlineFormatters[i].WritePlaintextOpening;
                inlinePlaintextClosers[i] = inlineFormatters[i].GetPlaintextClosing;
                inlineStackers[i] = inlineFormatters[i].IsWithinLink;
            }
        }

        public void BlocksToHtml(TextWriter writer, Block block)
        {
            var wrapper = new HtmlTextWriter(writer);
            BlocksToHtmlInner(wrapper, block);
        }

        private void BlocksToHtmlInner(HtmlTextWriter writer, Block block)
        {
            blockStack.Clear();
            inlineStack.Clear();

            bool visitChildren;
            string stackLiteral;
            bool renderHtmlInlines;
            bool tight = false;

            while (block != null)
            {
                visitChildren = WriteOpening(writer, block, tight);

                renderHtmlInlines = IsRenderHtmlInlines(block);
                if (renderHtmlInlines)
                {
                    InlinesToHtml(writer, block.InlineContent);
                    if (block.Tag == BlockTag.Paragraph)
                        writer.WriteConstant(GetClosing(block, tight));
                    else
                        writer.WriteLineConstant(GetClosing(block, tight));
                }

                if (visitChildren)
                {
                    stackLiteral = !renderHtmlInlines
                        ? GetClosing(block, tight)
                        : null;

                    blockStack.Push(new BlockStackEntry(stackLiteral, block.NextSibling, tight));

                    tight = IsTight(block, tight);
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

                while (block == null && blockStack.Count > 0)
                {
                    var entry = blockStack.Pop();

                    writer.WriteLineConstant(entry.Literal);
                    tight = entry.IsTight;
                    block = entry.Target;
                }
            }
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool WriteOpening(IHtmlTextWriter writer, Block block, bool tight)
        {
            return blockOpeners[(int)block.Tag](writer, block, tight);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private string GetClosing(Block block, bool tight)
        {
            return blockClosers[(int)block.Tag](block, tight);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsRenderHtmlInlines(Block block)
        {
            return blockHtmlRenderers[(int)block.Tag](block);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsTight(Block block, bool tight)
        {
            return blockStackers[(int)block.Tag](block, tight);
        }

        /// <summary>
        /// Writes the inline list to the given writer as HTML code. 
        /// </summary>
        private void InlinesToHtml(HtmlTextWriter writer, Inline inline)
        {
            bool stackPlaintext;
            bool? plaintext = false;
            bool withinLink = false;
            bool visitChildren;
            string stackLiteral;
            bool renderHtmlInlines = false;

            while (inline != null)
            {
                stackPlaintext = false;

                switch (plaintext)
                {
                    case false:
                        visitChildren = WriteOpening(writer, inline, withinLink);
                        stackLiteral = GetClosing(inline, withinLink);
                        stackPlaintext = IsRenderPlaintextInlines(inline);
                        renderHtmlInlines = IsRenderHtmlInlines(inline);
                        if (stackPlaintext)
                        {
                            plaintext = true;
                            visitChildren = true;
                        }
                        else if (renderHtmlInlines)
                        {
                            EscapeHtml(inline.LiteralContentValue, writer);
                        }
                        break;

                    case true:
                        visitChildren = WritePlaintextOpening(writer, inline, withinLink);
                        stackLiteral = GetPlaintextClosing(inline, withinLink);
                        break;

                    default:
                        visitChildren = false;
                        stackLiteral = null;
                        EscapeHtml(inline.LiteralContentValue, writer);
                        break;
                }

                if (visitChildren)
                {
                    inlineStack.Push(new InlineStackEntry(stackLiteral, inline.NextSibling, plaintext, withinLink));

                    if (stackPlaintext && renderHtmlInlines && inline.LiteralContentValue.Length > 0)
                        inlineStack.Push(new InlineStackEntry(GetInfix(inline), inline, null, withinLink));

                    withinLink = IsWithinLink(inline, withinLink);
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

                while (inline == null && inlineStack.Count > 0)
                {
                    var entry = inlineStack.Pop();
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
        private bool WriteOpening(IHtmlTextWriter writer, Inline inline, bool tight)
        {
            return inlineOpeners[(int)inline.Tag](writer, inline, tight);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private string GetInfix(Inline inline)
        {
            return inlineInfixers[(int)inline.Tag](inline);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private string GetClosing(Inline inline, bool withinLink)
        {
            return inlineClosers[(int)inline.Tag](inline, withinLink);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool WritePlaintextOpening(IHtmlTextWriter writer, Inline inline, bool tight)
        {
            return inlinePlaintextOpeners[(int)inline.Tag](writer, inline, tight);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private string GetPlaintextClosing(Inline inline, bool withinLink)
        {
            return inlinePlaintextClosers[(int)inline.Tag](inline, withinLink);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsRenderPlaintextInlines(Inline inline)
        {
            return inlinePlaintextRenderers[(int)inline.Tag](inline);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsRenderHtmlInlines(Inline inline)
        {
            return inlineHtmlRenderers[(int)inline.Tag](inline);
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsWithinLink(Inline inline, bool withinLink)
        {
            return inlineStackers[(int)inline.Tag](inline, withinLink);
        }

        private struct BlockStackEntry
        {
            public string Literal { get; }
            public Block Target { get; }
            public bool IsTight { get; }

            public BlockStackEntry(string literal, Block target, bool isTight)
            {
                Literal = literal;
                Target = target;
                IsTight = isTight;
            }
        }

        private struct InlineStackEntry
        {
            public string Literal { get; }
            public Inline Target { get; }
            public bool? IsPlaintext { get; }
            public bool IsWithinLink { get; }

            public InlineStackEntry(string literal, Inline target, bool? isPlaintext, bool isWithinLink)
            {
                Literal = literal;
                Target = target;
                IsPlaintext = isPlaintext;
                IsWithinLink = isWithinLink;
            }
        }
    }
}
