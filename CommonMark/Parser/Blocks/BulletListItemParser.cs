﻿using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Bullet list item delimiter parameters.
    /// </summary>
    public sealed class BulletListItemDelimiterParameters : ListItemDelimiterParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemDelimiterParameters"/> class.
        /// </summary>
        /// <param name="character">Delimiter character.</param>
        /// <param name="isHorizontalRuleCharacter"><c>true</c> if the delimiter character doubles as a horizontal rule character.</param>
        /// <param name="listStyle">List style.</param>
        /// <param name="minSpaceCount">Minimum space count.</param>
        public BulletListItemDelimiterParameters(char character, bool isHorizontalRuleCharacter = false, string listStyle = null, int minSpaceCount = 1)
            : base(character, minSpaceCount)
        {
            this.IsHorizontalRuleCharacter = isHorizontalRuleCharacter;
            this.ListStyle = listStyle;
        }

        /// <summary>
        /// Gets or sets the value indicating whether the delimiter character doubles as a horizontal rule character.
        /// </summary>
        public bool IsHorizontalRuleCharacter { get; set; }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        public string ListStyle { get; set; }
    }

    /// <summary>
    /// Bullet list item parameters.
    /// </summary>
    public sealed class BulletListItemParameters : ListItemParameters<BulletListItemDelimiterParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemParameters"/> class.
        /// </summary>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public BulletListItemParameters(BlockTag parentTag, ListType listType, params BulletListItemDelimiterParameters[] delimiters)
#pragma warning restore 0618
            : base(parentTag, listType, delimiters)
        {
        }
    }

    /// <summary>
    /// Bullet list item element parser.
    /// </summary>
    public sealed class BulletListItemHandler : ListItemHandler<BulletListData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        public BulletListItemHandler(CommonMarkSettings settings, BlockTag tag, BulletListItemParameters parameters, BulletListItemDelimiterParameters delimiter)
#pragma warning disable 0618
            : base(settings, tag, delimiter.Character, parameters, delimiter)
#pragma warning restore 0618
        {
            IsHorizontalRuleCharacter = delimiter.IsHorizontalRuleCharacter;
            ListStyle = delimiter.ListStyle;
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, IsListsMatch, SetListData);
        }

        /// <summary>
        /// Determines whether a list item can be opened.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if the current line may contain a handled list item element.</returns>
        protected override bool CanOpen(BlockParserInfo info)
        {
            return base.CanOpen(info) && !(IsHorizontalRuleCharacter && ScanHorizontalRule(info, info.CurrentCharacter));
        }

        /// <summary>
        /// Determines whether a list item belongs to a matching bullet list.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Bullet list data.</param>
        /// <returns><c>true</c> if the container may continue a list having <paramref name="listData"/>.</returns>
        protected override bool IsListsMatch(BlockParserInfo info, BulletListData listData)
        {
            return listData.Equals(info.Container.BulletListData);
        }

        /// <summary>
        /// Updates a container with bullet list data.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="listData">Bullet list data.</param>
        protected override void SetListData(BlockParserInfo info, BulletListData listData)
        {
            info.Container.BulletListData = listData;
        }

        /// <summary>
        /// Attempts to parse a bullet list item marker.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <param name="data">Common list data.</param>
        /// <param name="listData">Bullet list data.</param>
        /// <returns>Length of the marker, or 0 for no match.</returns>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        protected override int ParseMarker(BlockParserInfo info, out ListData data, out BulletListData listData)
        {
            data = null;
            listData = null;

            var curChar = info.CurrentCharacter;
            var line = info.Line;
            var length = line.Length;
            var offset = info.FirstNonspace;

            if (offset == length - 1)
                return 0;

            return CompleteScan(info, offset, null, curChar, curChar, '\0',
                GetStart, GetListData, out data, out listData);
        }

        /// <summary>
        /// Calculates an integer start value.
        /// </summary>
        /// <param name="startStr">Start string.</param>
        /// <returns>Integer start value, or 1 if unsuccessful.</returns>
        protected override int GetStart(string startStr)
        {
            return 1;
        }

        /// <summary>
        /// Creates and initializes bullet list data.
        /// </summary>
        /// <param name="curChar">Current character.</param>
        /// <param name="start">Start value.</param>
        /// <returns></returns>
        protected override BulletListData GetListData(char curChar, int start)
        {
            return new BulletListData
            {
                BulletCharacter = curChar,
                ListStyle = ListStyle,
            };
        }

        private bool IsHorizontalRuleCharacter
        {
            get;
        }

        private string ListStyle
        {
            get;
        }
    }
}
