using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal static class Printer
    {
        internal static string format_str(string s, StringBuilder buffer)
        {
            if (s == null)
                return string.Empty;

            int pos = 0;
            int len = s.Length;
            char c;

            buffer.Length = 0;
            buffer.Append('\"');
            while (pos < len)
            {
                c = s[pos];
                switch (c)
                {
                    case '\n':
                        buffer.Append("\\n");
                        break;
                    case '"':
                        buffer.Append("\\\"");
                        break;
                    case '\\':
                        buffer.Append("\\\\");
                        break;
                    default:
                        buffer.Append(c);
                        break;
                }
                pos++;
            }
            buffer.Append('\"');
            return buffer.ToString();
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static void PrintPosition(bool enabled, TextWriter writer, Block block)
        {
            if (enabled)
            {
                writer.Write(" [");
                writer.Write(block.SourcePosition);
                writer.Write('-');
                writer.Write(block.SourceLastPosition);
                writer.Write(']');
            }
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static void PrintPosition(bool enabled, TextWriter writer, Inline inline)
        {
            if (enabled)
            {
                writer.Write(" [");
                writer.Write(inline.SourcePosition);
                writer.Write('-');
                writer.Write(inline.SourceLastPosition);
                writer.Write(']');
            }
        }

        /// <summary>
        /// Write the block data to the given writer.
        /// </summary>
        public static void PrintBlocks(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            int indent = 0;
            var stack = new Stack<BlockStackEntry>();
            var inlineStack = new Stack<InlineStackEntry>();
            var buffer = new StringBuilder();
            var trackPositions = settings.TrackSourcePosition;

            var parameters = settings.FormatterParameters;
            IBlockFormatter[] formatters = parameters.BlockFormatters;
            IBlockFormatter formatter;
            IDictionary<string, object> data;

            while (block != null)
            {
                writer.Write(new string(' ', indent));

                formatter = formatters[(int)block.Tag];
                if (formatter != null)
                {
                    writer.Write(formatter.PrinterTag);
                    PrintPosition(trackPositions, writer, block);
                    if ((data = formatter.GetPrinterData(parameters.Printer, block)) != null)
                        PrintData(writer, data);
                }
                else switch (block.Tag)
                    {
                        case BlockTag.Document:
                            writer.Write("document");
                            PrintPosition(trackPositions, writer, block);
                            break;

                        case BlockTag.BlockQuote:
                            writer.Write("block_quote");
                            PrintPosition(trackPositions, writer, block);
                            break;

                        case BlockTag.Paragraph:
                            writer.Write("paragraph");
                            PrintPosition(trackPositions, writer, block);
                            break;

                        case BlockTag.HtmlBlock:
                            writer.Write("html_block");
                            PrintPosition(trackPositions, writer, block);
                            writer.Write(' ');
                            writer.Write(format_str(block.StringContent.ToString(buffer), buffer));
                            break;

                        case BlockTag.ReferenceDefinition:
                            writer.Write("reference_def");
                            PrintPosition(trackPositions, writer, block);
                            break;

                        default:
                            throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                    }

                writer.WriteLine();

                if (block.InlineContent != null)
                {
                    PrintInlines(writer, block.InlineContent, indent + 2, inlineStack, buffer, settings);
                }

                if (block.FirstChild != null)
                {
                    if (block.NextSibling != null)
                        stack.Push(new BlockStackEntry(indent, block.NextSibling));

                    indent += 2;
                    block = block.FirstChild;
                }
                else if (block.NextSibling != null)
                {
                    block = block.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    var entry = stack.Pop();
                    indent = entry.Indent;
                    block = entry.Target;
                }
                else
                {
                    block = null;
                }
            }
        }

        private static void PrintInlines(TextWriter writer, Inline inline, int indent, Stack<InlineStackEntry> stack, StringBuilder buffer, CommonMarkSettings settings)
        {
            var trackPositions = settings.TrackSourcePosition;
            var parameters = settings.FormatterParameters;
            IInlineFormatter[] formatters = parameters.InlineFormatters;
            IInlineFormatter formatter;
            IDictionary<string, object> data;

            while (inline != null)
            {
                writer.Write(new string(' ', indent));

                formatter = formatters[(int)inline.Tag];
                if (formatter != null)
                {
                    writer.Write(formatter.PrinterTag);
                    PrintPosition(trackPositions, writer, inline);
                    if ((data = formatter.GetPrinterData(parameters.Printer, inline)) != null)
                        PrintData(writer, data);
                }
                else switch (inline.Tag)
                    {
                        case InlineTag.String:
                            writer.Write("str");
                            PrintPosition(trackPositions, writer, inline);
                            writer.Write(' ');
                            writer.Write(format_str(inline.LiteralContent, buffer));
                            break;

                        case InlineTag.SoftBreak:
                            writer.Write("softbreak");
                            PrintPosition(trackPositions, writer, inline);
                            break;

                        case InlineTag.Code:
                            writer.Write("code {0}", format_str(inline.LiteralContent, buffer));
                            PrintPosition(trackPositions, writer, inline);
                            writer.Write(' ');
                            writer.Write(format_str(inline.LiteralContent, buffer));
                            break;

                        case InlineTag.RawHtml:
                            writer.Write("html {0}", format_str(inline.LiteralContent, buffer));
                            writer.Write(' ');
                            writer.Write(format_str(inline.LiteralContent, buffer));
                            break;

                        case InlineTag.Image:
                            writer.Write("image");
                            PrintPosition(trackPositions, writer, inline);
                            writer.Write(" url={0} title={1}",
                                   format_str(inline.TargetUrl, buffer),
                                   format_str(inline.LiteralContent, buffer));
                            break;

                        case InlineTag.Strong:
                            writer.Write("strong");
                            PrintPosition(trackPositions, writer, inline);
                            break;

                        case InlineTag.Emphasis:
                            writer.Write("emph");
                            PrintPosition(trackPositions, writer, inline);
                            break;

                        default:
                            writer.Write("unknown: " + inline.Tag.ToString());
                            PrintPosition(trackPositions, writer, inline);
                            break;
                    }

                writer.WriteLine();

                if (inline.FirstChild != null)
                {
                    if (inline.NextSibling != null)
                        stack.Push(new InlineStackEntry(indent, inline.NextSibling));

                    indent += 2;
                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    var entry = stack.Pop();
                    indent = entry.Indent;
                    inline = entry.Target;
                }
                else
                {
                    inline = null;
                }
            }
        }

        private static void PrintData(TextWriter writer, IDictionary<string, object> data)
        {
            if (data.Count > 1 || !data.ContainsKey(string.Empty))
                writer.Write(" (");
            foreach (var kvp in data)
            {
                if (kvp.Key.Length == 0)
                    writer.Write(" {0}", kvp.Value);
                else
                    writer.Write(" {0}={1}", kvp.Key, kvp.Value);
            }
            if (data.Count > 1 || !data.ContainsKey(string.Empty))
                writer.Write(')');
        }

        private struct BlockStackEntry
        {
            public readonly int Indent;
            public readonly Block Target;
            public BlockStackEntry(int indent, Block target)
            {
                this.Indent = indent;
                this.Target = target;
            }
        }
        private struct InlineStackEntry
        {
            public readonly int Indent;
            public readonly Inline Target;
            public InlineStackEntry(int indent, Inline target)
            {
                this.Indent = indent;
                this.Target = target;
            }
        }
    }

    internal class PrinterImpl : IPrinter
    {

        private StringBuilder buffer;

        internal PrinterImpl()
        {
            this.buffer = new StringBuilder();
        }

        string IPrinter.Format(string s)
        {
            return Printer.format_str(s, buffer);
        }

        string IPrinter.Format(StringContent stringContent)
        {
            return ((IPrinter)this).Format(stringContent.ToString(buffer));
        }
    }
}
