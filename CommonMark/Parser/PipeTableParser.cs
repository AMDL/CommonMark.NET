using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal sealed class PipeTableParser
    {
        private readonly CommonMarkSettings settings;

        public PipeTableParser(CommonMarkSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Match pipe table header line.
        /// </summary>
        /// <returns>Column count, or 0 for no match.</returns>
        internal int ScanHeaderLine(string s, int pos, int sourceLength, out TableData data)
        {
            data = null;

            if (pos >= sourceLength || !IsHeaderOpener(s[pos]))
                return 0;

            data = new TableData
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
                            return 0;
                    }
                    else
                        data.HeaderDelimiter = c;

                    if (charCount == 0)
                    {
                        if (columnData != null)
                            return 0;
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
                            return 0;
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
                            return 0;
                    }
                    else
                        data.HeaderColumnDelimiter = c;

                    if (charCount == 0)
                    {
                        if (data.FirstColumn != null || !IsColumnDelimiter(c))
                            return 0;
                        columnData = new TableColumnData();
                        continue;
                    }

                    if (columnData == null || charCount < 3)
                        return 0;

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

                return 0;
            }

            if (columnData == null)
            {
                if (!IsColumnDelimiter(data.HeaderColumnDelimiter))
                    return 0;
            }
            else
            {
                if (charCount > 0 && charCount < 3)
                    return 0;

                if (data.FirstColumn == null)
                    return 0;
                data.LastColumn.NextSibling = columnData;
                data.LastColumn = columnData;
                data.ColumnCount++;
            }

            if (data.FirstColumn == data.LastColumn)
                return 0;

            return data.ColumnCount;
        }

        internal static void IncorporateCells(Block block)
        {
            if (block.Tag != BlockTag.TableCell)
                return;

            var tableData = block.Parent.TableData;
            var columnData = tableData.FirstColumn;
            var cellType = block.TableRowData.TableRowType == TableRowType.Header
                ? TableCellType.Header
                : TableCellType.Default;
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
                        CellType = cellType,
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
        /// Checks if a character can serve as a pipe table header line opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can open a pipe table header line.</returns>
        private bool IsHeaderOpener(char c)
        {
            return (0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.PipeTables))
                && (IsHeaderDelimiter(c) || IsColumnDelimiter(c) || IsHeaderAlignmentMarker(c));
        }

        /// <summary>
        /// Checks if a character can serve as a pipe table header line delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a pipe table header line delimiter.
        /// </returns>
        private bool IsHeaderDelimiter(char c)
        {
            return c == '-' || (c == '=' && 0 != (settings.Tables.PipeTables & PipeTableFeatures.HeaderEquals));
        }

        /// <summary>
        /// Checks if a character can serve as a column delimiter in a pipe table header line.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a column delimiter in a pipe table header line.
        /// </returns>
        private bool IsHeaderColumnDelimiter(char c)
        {
            return IsColumnDelimiter(c) || (c == '+' && 0 != (settings.Tables.PipeTables & PipeTableFeatures.HeaderPlus));
        }

        /// <summary>
        /// Checks if a character can serve both as a column delimiter and as a header line opener in a pipe table.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a pipe table column delimiter and can open a pipe table header line.
        /// </returns>
        private bool IsColumnDelimiter(char c)
        {
            return c == '|';
        }

        /// <summary>
        /// Checks if a character can serve as a pipe table header alignment marker.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as an alignment marker in a pipe table header line.
        /// </returns>
        private bool IsHeaderAlignmentMarker(char c)
        {
            return (c == ':' && 0 != (settings.Tables.PipeTables & PipeTableFeatures.HeaderColon));
        }
    }
}
