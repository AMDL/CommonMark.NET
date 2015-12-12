using CommonMark.Extension;
using CommonMark.Parser;
using CommonMark.Syntax;

namespace CommonMark.Extension
{
    /// <summary>
    /// Pipe tables.
    /// </summary>
    public sealed class PipeTables : CommonMarkExtension
    {
        private readonly PipeTablesSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTables"/> class.
        /// </summary>
        /// <param name="settings">Pipe tables settings.</param>
        public PipeTables(PipeTablesSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Processes a single line, optionally modifying a block.
        /// </summary>
        /// <param name="container">Container element.</param>
        /// <param name="line">Line string.</param>
        /// <param name="first_nonspace">The index of the first non-space character.</param>
        /// <param name="indented"><c>true</c> if the line is indented.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="column">Column index.</param>
        /// <returns><c>true</c> if successful.</returns>
        public bool IncorporateLine(Block container, string line, int first_nonspace, bool indented, ref int offset, ref int column)
        {
            TableData data;
            if (!indented && container.Tag == BlockTag.Paragraph && BlockMethods.ContainsSingleLine(container.StringContent)
                && null != (data = ScanHeaderLine(line, first_nonspace, line.Length)))
            {
                container.Tag = BlockTag.Table;
                container.TableData = data;
                BlockMethods.AdvanceOffset(line, line.Length - 1 - offset, false, ref offset, ref column);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Match pipe table header line.
        /// </summary>
        /// <returns>Table data, or <c>null</c> for no match.</returns>
        private TableData ScanHeaderLine(string s, int pos, int sourceLength)
        {
            if (pos >= sourceLength || !IsHeaderOpener(s[pos]))
                return null;

            var data = new TableData
            {
                TableType = TableType.Pipe,
                ColumnDelimiter = '|',
            };

            var charCount = 0;
            TableColumnData columnData = null;

            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (IsHeaderDelimiter(c))
                {
                    if (data.HeaderDelimiter != 0)
                    {
                        if (c != data.HeaderDelimiter)
                            return null;
                    }
                    else
                        data.HeaderDelimiter = c;

                    if (charCount == 0)
                    {
                        if (columnData != null)
                            return null;
                        columnData = new TableColumnData();
                    }

                    charCount++;
                    continue;
                }

                if (IsHeaderAlignmentMarker(c))
                {
                    if (charCount == 0)
                    {
                        columnData = new TableColumnData();
                        columnData.Alignment = TableColumnAlignment.Left;
                    }
                    else
                    {
                        if (charCount < 2 || (columnData.Alignment & TableColumnAlignment.Right) != 0)
                            return null;
                        columnData.Alignment |= TableColumnAlignment.Right;
                    }

                    charCount++;
                    continue;
                }

                if (IsHeaderColumnDelimiter(c))
                {
                    if (data.HeaderColumnDelimiter != 0)
                    {
                        if (c != data.HeaderColumnDelimiter)
                            return null;
                    }
                    else
                        data.HeaderColumnDelimiter = c;

                    if (charCount == 0)
                    {
                        if (data.FirstColumn != null || !IsColumnDelimiter(c))
                            return null;
                        columnData = new TableColumnData();
                        continue;
                    }

                    if (columnData == null || charCount < 3)
                        return null;

                    if (data.FirstColumn == null)
                        data.FirstColumn = columnData;
                    else
                        data.LastColumn.NextSibling = columnData;
                    data.LastColumn = columnData;
                    data.ColumnCount++;
                    columnData = null;
                    charCount = 0;
                    continue;
                }

                charCount = 0;

                if (c == ' ')
                    continue;

                if (c == '\n')
                    break;

                return null;
            }

            if (columnData == null)
            {
                if (!IsColumnDelimiter(data.HeaderColumnDelimiter))
                    return null;
            }
            else
            {
                if (charCount > 0 && charCount < 3)
                    return null;

                if (data.FirstColumn == null)
                    return null;
                data.LastColumn.NextSibling = columnData;
                data.LastColumn = columnData;
                data.ColumnCount++;
            }

            if (data.FirstColumn == data.LastColumn)
                return null;

            return data;
        }

        private static void IncorporateHeader(Block block, int startLine, int startColumn, int sourcePosition)
        {
            var lastChild = block.LastChild;
#pragma warning disable 0618
            block.LastChild = new Block(BlockTag.TableHeader, startLine, startColumn, sourcePosition)
#pragma warning restore 0618
            {
                Parent = block,
#pragma warning disable 0618
                Previous = lastChild,
#pragma warning restore 0618
                StringContent = block.StringContent,
            };

            if (lastChild != null)
                lastChild.NextSibling = block.LastChild;

            block.StringContent = null;
        }

        private static void IncorporateFooter(Block block, int startLine, int startColumn, int sourcePosition)
        {
            var lastChild = block.LastChild;
#pragma warning disable 0618
            block.LastChild = new Block(BlockTag.TableFooter, startLine, startColumn, sourcePosition)
#pragma warning restore 0618
            {
                Parent = block,
#pragma warning disable 0618
                Previous = lastChild,
#pragma warning restore 0618
                StringContent = block.StringContent,
            };

            if (lastChild != null)
                lastChild.NextSibling = block.LastChild;

            block.StringContent = null;
        }

        private static void IncorportateBody(Block block, int startLine, int startColumn, int sourcePosition)
        {
            var lastChild = block.LastChild;
#pragma warning disable 0618
            block.LastChild = new Block(BlockTag.TableBody, startLine, startColumn, sourcePosition)
#pragma warning restore 0618
            {
                Parent = block,
#pragma warning disable 0618
                Previous = lastChild,
#pragma warning restore 0618
                StringContent = block.StringContent,
            };

            if (lastChild != null)
                lastChild.NextSibling = block.LastChild;

            block.StringContent = null;
        }

        private static void IncorporateRow(Block block, int startLine, int startColumn, int sourcePosition)
        {
            var lastChild = block.LastChild;
#pragma warning disable 0618
            block.LastChild = new Block(BlockTag.TableRow, startLine, startColumn, sourcePosition)
#pragma warning restore 0618
            {
                Parent = block,
#pragma warning disable 0618
                Previous = lastChild,
#pragma warning restore 0618
                TableRowData = new TableRowData(),
                StringContent = block.StringContent,
            };

            if (lastChild != null)
                lastChild.NextSibling = block.LastChild;

            block.StringContent = null;
        }

        internal static void IncorporateCells(Block block)
        {
            if (block.Tag != BlockTag.TableCell)
                return;

            var tableData = block.Parent.TableData;
            var columnData = tableData.FirstColumn;
            var cellCount = 0;

            var sourcePosition = block.SourcePosition;
#pragma warning disable 0618
            var startLine = block.StartLine;
            var startColumn = block.StartColumn;
#pragma warning restore 0618

            Block child = null;
            Block nextChild;
            Inline inline = block.InlineContent;
            Inline nextInline;

            while (inline != null || columnData != null)
            {
                if (inline != null)
                {
                    sourcePosition = inline.SourcePosition;
                    //startLine = inline.StartLine;
                    //startColumn = inline.StartColumn;
                }

#pragma warning disable 0618
                nextChild = new Block(BlockTag.TableCell, startLine, startColumn, sourcePosition)
#pragma warning restore 0618
                {
                    Parent = block,
                    TableCellData = new TableCellData
                    {
                        ColumnData = columnData ?? new TableColumnData()
                    },
                    InlineContent = inline,
                };

                cellCount++;

                if (child == null)
                    block.FirstChild = nextChild;
                else
                    child.NextSibling = nextChild;

                child = nextChild;

                if (inline != null)
                {
                    nextInline = inline.NextSibling;
                    inline.NextSibling = null;
                    inline = nextInline;
                }

                if (columnData != null)
                    columnData = columnData.NextSibling;
            }

            block.TableRowData.CellCount = cellCount;
            block.InlineContent = null;
        }

        /// <summary>
        /// Checks if a character can serve as a header line opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can open a header line.</returns>
        private bool IsHeaderOpener(char c)
        {
            return IsHeaderDelimiter(c) || IsColumnDelimiter(c) || IsHeaderAlignmentMarker(c);
        }

        /// <summary>
        /// Checks if a character can serve as a header line delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a header line delimiter.
        /// </returns>
        private bool IsHeaderDelimiter(char c)
        {
            return c == '-' || (c == '=' && 0 != (settings.Features & PipeTablesFeatures.HeaderEquals));
        }

        /// <summary>
        /// Checks if a character can serve as a column delimiter in a header line.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a column delimiter in a header line.
        /// </returns>
        private bool IsHeaderColumnDelimiter(char c)
        {
            return IsColumnDelimiter(c) || (c == '+' && 0 != (settings.Features & PipeTablesFeatures.HeaderPlus));
        }

        /// <summary>
        /// Checks if a character can serve both as a column delimiter and as a header line opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a column delimiter and can open a header line.
        /// </returns>
        private bool IsColumnDelimiter(char c)
        {
            return c == '|';
        }

        /// <summary>
        /// Checks if a character can serve as a header alignment marker.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as an alignment marker in a header line.
        /// </returns>
        private bool IsHeaderAlignmentMarker(char c)
        {
            return (c == ':' && 0 != (settings.Features & PipeTablesFeatures.HeaderColon));
        }
    }
}
