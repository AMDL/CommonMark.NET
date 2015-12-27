using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal static class TextSyntaxFormatter
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

        /// <summary>
        /// Write the block data to the given writer.
        /// </summary>
        public static void PrintBlocks(TextWriter writer, Block element, CommonMarkSettings settings)
        {
            int indent = 0;
            var stack = new Stack<BlockStackEntry>();
            var inlineStack = new Stack<InlineStackEntry>();
            var buffer = new StringBuilder();
            var trackPositions = settings.TrackSourcePosition;

            var parameters = settings.FormatterParameters;
            IBlockFormatter[] formatters = parameters.BlockFormatters;
            IBlockFormatter formatter;
            IEnumerable<KeyValuePair<string, object>> data;

            while (element != null)
            {
                writer.Write(new string(' ', indent));

                formatter = formatters[(int)element.Tag];
                if (formatter == null)
                {
                    throw new CommonMarkException("Block type " + element.Tag + " is not supported.", element);
                }

                writer.Write(formatter.TextTag);
                if (trackPositions)
                {
                    writer.Write(" [");
                    writer.Write(element.SourcePosition);
                    writer.Write('-');
                    writer.Write(element.SourceLastPosition);
                    writer.Write(']');
                }
                if ((data = formatter.GetSyntaxData(parameters.SyntaxFormatter, element)) != null)
                    PrintData(writer, data);

                writer.WriteLine();

                if (element.InlineContent != null)
                {
                    PrintInlines(writer, element.InlineContent, indent + 2, inlineStack, buffer, settings);
                }

                if (element.FirstChild != null)
                {
                    if (element.NextSibling != null)
                        stack.Push(new BlockStackEntry(indent, element.NextSibling));

                    indent += 2;
                    element = element.FirstChild;
                }
                else if (element.NextSibling != null)
                {
                    element = element.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    var entry = stack.Pop();
                    indent = entry.Indent;
                    element = entry.Target;
                }
                else
                {
                    element = null;
                }
            }
        }

        private static void PrintInlines(TextWriter writer, Inline element, int indent, Stack<InlineStackEntry> stack, StringBuilder buffer, CommonMarkSettings settings)
        {
            var trackPositions = settings.TrackSourcePosition;
            var parameters = settings.FormatterParameters;
            IInlineFormatter[] formatters = parameters.InlineFormatters;
            IInlineFormatter formatter;
            IEnumerable<KeyValuePair<string, object>> data;

            while (element != null)
            {
                writer.Write(new string(' ', indent));

                formatter = formatters[(int)element.Tag];
                if (formatter == null)
                {
                    throw new CommonMarkException("Inline type " + element.Tag + " is not supported.", element);
                }

                writer.Write(formatter.TextTag);
                if (trackPositions)
                {
                    writer.Write(" [");
                    writer.Write(element.SourcePosition);
                    writer.Write('-');
                    writer.Write(element.SourceLastPosition);
                    writer.Write(']');
                }
                if ((data = formatter.GetSyntaxData(parameters.SyntaxFormatter, element)) != null)
                    PrintData(writer, data);

                writer.WriteLine();

                if (element.FirstChild != null)
                {
                    if (element.NextSibling != null)
                        stack.Push(new InlineStackEntry(indent, element.NextSibling));

                    indent += 2;
                    element = element.FirstChild;
                }
                else if (element.NextSibling != null)
                {
                    element = element.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    var entry = stack.Pop();
                    indent = entry.Indent;
                    element = entry.Target;
                }
                else
                {
                    element = null;
                }
            }
        }

        private static void PrintData(TextWriter writer, IEnumerable<KeyValuePair<string, object>> data)
        {
            var multi = IsMultiValue(data);
            if (multi)
                writer.Write(" (");
            foreach (var kvp in data)
            {
                if (kvp.Key.Length == 0)
                    writer.Write(" {0}", kvp.Value);
                else
                    writer.Write(" {0}={1}", kvp.Key, kvp.Value);
            }
            if (multi)
                writer.Write(')');
        }

        private static bool IsMultiValue(IEnumerable<KeyValuePair<string, object>> data)
        {
            var count = 0;
            var containsEmptyKey = false;
            foreach (var kvp in data)
            {
                if (++count > 1)
                    return true;
                if (kvp.Key.Length == 0)
                    containsEmptyKey = true;
            }
            return !containsEmptyKey;
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
}
