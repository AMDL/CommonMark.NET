using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal sealed class TableParser
    {
        private readonly CommonMarkSettings settings;

        public TableParser(CommonMarkSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Match table header line.
        /// </summary>
        /// <returns>Column count, or 0 for no match.</returns>
        internal int ScanTableHeaderLine(string s, int pos, int sourceLength, out TableData data)
        {
            data = null;

            if (pos >= sourceLength || !IsPipeTableOpener(s[pos]))
                return 0;

            var columnList = new System.Collections.Generic.List<TableColumnData>();
            var charCount = 0;
            char columnDelimiter = (char)0;
            TableColumnData columnData = null;

            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];

                if (IsPipeTableHeaderDelimiter(c))
                {
                    if (charCount == 0)
                    {
                        if (columnData != null)
                            return 0;
                        columnData = new TableColumnData();
                    }

                    charCount++;
                    continue;
                }

                if (IsPipeTableHeaderAlignmentMarker(c))
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

                if (IsPipeTableHeaderColumnDelimiter(c))
                {
                    if (columnDelimiter != 0)
                    {
                        if (c != columnDelimiter)
                            return 0;
                    }
                    else
                        columnDelimiter = c;

                    if (charCount == 0)
                    {
                        if (columnList.Count > 0 || !IsPipeTableColumnDelimiter(c))
                            return 0;
                        columnData = new TableColumnData();
                        continue;
                    }

                    if (columnData == null || charCount < 3)
                        return 0;

                    columnList.Add(columnData);
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
                if (!IsPipeTableColumnDelimiter(columnDelimiter))
                    return 0;
            }
            else
            {
                if (charCount > 0 && charCount < 3)
                    return 0;
                columnList.Add(columnData);
            }

            if (columnList.Count <= 1)
                return 0;

            data = new TableData
            {
                TableType = TableType.Pipe,
                ColumnData = columnList.ToArray(),
                HeaderColumnDelimiter = columnDelimiter == '|' ? TableHeaderColumnDelimiter.Pipe : TableHeaderColumnDelimiter.Plus,
                ColumnDelimiter = TableColumnDelimiter.Pipe,
            };

            return columnList.Count;
        }

        internal static void IncorporateCells(Block block)
        {
            if (block.Tag != BlockTag.TableCell)
                return;

            var tableData = block.Parent.TableData;
            var columnData = tableData.ColumnData;
            var columnCount = columnData.Length;
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

            while (inline != null || cellCount < columnCount)
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
                        ColumnData = cellCount < columnCount
                            ? columnData[cellCount]
                            : new TableColumnData()
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
            }

            block.TableRowData.CellCount = cellCount;
            block.InlineContent = null;
        }

        /// <summary>
        /// Checks if a character can serve as a pipe table header line opener.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the character can open a pipe table header line.</returns>
        private bool IsPipeTableOpener(char c)
        {
            return (0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.PipeTables))
                && (IsPipeTableHeaderDelimiter(c) || IsPipeTableColumnDelimiter(c) || IsPipeTableHeaderAlignmentMarker(c));
        }

        /// <summary>
        /// Checks if a character can serve as a pipe table header line delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a pipe table header line delimiter.
        /// </returns>
        private bool IsPipeTableHeaderDelimiter(char c)
        {
            return c == '-';
        }

        /// <summary>
        /// Checks if a character can serve as a column delimiter in a pipe table header line.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a column delimiter in a pipe table header line.
        /// </returns>
        private bool IsPipeTableHeaderColumnDelimiter(char c)
        {
            return IsPipeTableColumnDelimiter(c) || (c == '+' && 0 != (settings.PipeTables & PipeTableFeatures.HeaderPlus));
        }

        /// <summary>
        /// Checks if a character can serve both as a column delimiter and as a header line opener in a pipe table.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>
        /// <c>true</c> if the character can be used as a pipe table column delimiter and can open a pipe table header line.
        /// </returns>
        private bool IsPipeTableColumnDelimiter(char c)
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
        private bool IsPipeTableHeaderAlignmentMarker(char c)
        {
            return (c == ':' && 0 != (settings.PipeTables & PipeTableFeatures.HeaderColon));
        }
    }
}
