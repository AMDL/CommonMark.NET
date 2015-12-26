using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// List item parameters.
    /// </summary>
    public interface IListItemParameters
    {
        /// <summary>
        /// Gets the list item element tag.
        /// </summary>
        BlockTag Tag { get; }

        /// <summary>
        /// Gets the list element tag.
        /// </summary>
        BlockTag ParentTag { get; }

        /// <summary>
        /// Gets the list type.
        /// </summary>
        [Obsolete("This API has been superseded by " + nameof(BlockTag.UnorderedList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        ListType ListType { get; }
    }

    /// <summary>
    /// Base list item parameters class.
    /// </summary>
    /// <typeparam name="TDelimiterParameters">Type of delimiter parameters.</typeparam>
    public abstract class ListItemParameters<TDelimiterParameters> : IListItemParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParameters{TDelimiterParameters}"/> class.
        /// </summary>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        protected ListItemParameters(BlockTag tag, BlockTag parentTag, ListType listType, TDelimiterParameters[] delimiters)
        {
            this.Tag = tag;
            this.ParentTag = parentTag;
            this.ListType = listType;
            this.Delimiters = delimiters;
        }
#pragma warning restore 0618

        /// <summary>
        /// Gets or sets the list item element tag.
        /// </summary>
        public BlockTag Tag { get; set; }

        /// <summary>
        /// Gets or sets the list element tag.
        /// </summary>
        public BlockTag ParentTag { get; set; }

        /// <summary>
        /// Gets or sets the list type.
        /// </summary>
        [Obsolete("This API has been superseded by " + nameof(BlockTag.UnorderedList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        public ListType ListType { get; set; }

        /// <summary>
        /// Gets or sets the delimiter parameters.
        /// </summary>
        public TDelimiterParameters[] Delimiters { get; set; }
    }

    /// <summary>
    /// List parameters.
    /// </summary>
    public sealed class ListParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListParameters"/> class.
        /// </summary>
        /// <param name="items">List item parameters.</param>
        public ListParameters(params IListItemParameters[] items)
        {
            this.Items = items;
        }

        /// <summary>
        /// Gets the list item parameters.
        /// </summary>
        public IListItemParameters[] Items { get; set; }
    }

    /// <summary>
    /// Base <see cref="BlockTag.ListItem"/> element parser class.
    /// </summary>
    public sealed class ListItemParser : BlockParser
    {
        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly ListParameters DefaultParameters = new ListParameters(
            UnorderedListItemHandler.DefaultParameters,
            NumericListItemHandler.DefaultParameters);

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="tag">Handled element tag.</param>
        /// <param name="parameters">List parameters.</param>
        public ListItemParser(CommonMarkSettings settings, BlockTag tag = BlockTag.ListItem, ListParameters parameters = null)
            : base(settings, tag)
        {
            Parameters = parameters ?? DefaultParameters;
        }

        /// <summary>
        /// Gets the block element delimiter handlers.
        /// </summary>
        public override IEnumerable<IBlockDelimiterHandler> Handlers
        {
            get
            {
                foreach (var item in Parameters.Items)
                {
                    var unorderedParameters = item as UnorderedListItemParameters;
                    if (unorderedParameters != null)
                        yield return UnorderedListItemHandler.Create(Settings, unorderedParameters);

                    var numericParameters = item as OrderedListItemParameters;
                    if (numericParameters != null && numericParameters.Markers.Length == 1)
                        yield return NumericListItemHandler.Create(Settings, numericParameters);
                }
            }
        }

        /// <summary>
        /// Gets the list parameters.
        /// </summary>
        public ListParameters Parameters
        {
            get;
        }

        /// <summary>
        /// Determines whether the last blank line of the handled element should be discarded.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if a blank line at the end of the handled element should be discarded.</returns>
        public override bool IsDiscardLastBlank(BlockParserInfo info)
        {
            // we don't set IsLastLineBlank on an empty list item.
            return info.Container.FirstChild == null
                && info.Container.SourcePosition >= info.LineInfo.LineOffset;
        }

        /// <summary>
        /// Determines whether a handled element can contain child elements of the specified kind.
        /// </summary>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if handled elements can contain elements having <paramref name="childTag"/>.</returns>
        public override bool CanContain(BlockTag childTag)
        {
            return true;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            switch (info.Container.Parent.Tag)
            {
                case BlockTag.UnorderedList:
                    return DoInitialize(info, info.Container.UnorderedListData);
                case BlockTag.OrderedList:
                    return DoInitialize(info, info.Container.OrderedListData);
                default:
#pragma warning disable 0618
                    return DoInitialize(info, info.Container.ListData);
#pragma warning restore 0618
            }
        }

        private static bool DoInitialize<TData>(BlockParserInfo info, TData listData)
            where TData : ListData<TData>
        {
            var count = listData.MarkerOffset + listData.Padding;
            if (info.Indent >= count)
            {
                info.AdvanceOffset(count, true);
                return true;
            }
            if (info.IsBlank && info.Container.FirstChild != null)
            {
                // if container.FirstChild is null, then the opening line
                // of the list item was blank after the list marker; in this
                // case, we are done with the list item.
                info.AdvanceOffset(info.FirstNonspace - info.Offset, false);
                return true;
            }
            return false;
        }
    }
}
