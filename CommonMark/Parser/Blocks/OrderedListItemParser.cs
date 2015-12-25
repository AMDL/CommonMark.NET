using CommonMark.Parser.Blocks.Delimiters;
using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// Base ordered list marker parameters class.
    /// </summary>
    public abstract class OrderedListMarkerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListMarkerParameters"/> class.
        /// </summary>
        /// <param name="startValue">Start value.</param>
        protected OrderedListMarkerParameters(int startValue)
        {
            this.StartValue = startValue;
        }

        /// <summary>
        /// Gets the start value (0 for decimal numerals).
        /// </summary>
        public int StartValue { get; }
    }

    /// <summary>
    /// Ordered list single marker parameters.
    /// </summary>
    class OrderedListSingleMarkerParameters : OrderedListMarkerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListSingleMarkerParameters"/> class.
        /// </summary>
        /// <param name="character">Marker character.</param>
        /// <param name="value">Value.</param>
        public OrderedListSingleMarkerParameters(char character, int value)
            : base(value)
        {
            this.Character = character;
        }

        /// <summary>
        /// Gets or sets the marker character.
        /// </summary>
        public char Character { get; set; }
    }

    /// <summary>
    /// Ordered list marker range parameters.
    /// </summary>
    public sealed class OrderedListMarkerRangeParameters : OrderedListMarkerParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListMarkerRangeParameters"/> class.
        /// </summary>
        /// <param name="minChar">First marker character.</param>
        /// <param name="maxChar">Last marker character.</param>
        /// <param name="startValue">Start value.</param>
        public OrderedListMarkerRangeParameters(char minChar, char maxChar, int startValue = 0)
            : base(startValue)
        {
            this.MinCharacter = minChar;
            this.MaxCharacter = maxChar;
        }

        /// <summary>
        /// Gets or sets the first character in the list marker character range.
        /// </summary>
        public char MinCharacter { get; set; }

        /// <summary>
        /// Gets or sets the last character in the list marker character range.
        /// </summary>
        public char MaxCharacter { get; set; }
    }

    /// <summary>
    /// Ordered list item parameters.
    /// </summary>
    public sealed class OrderedListItemParameters : ListItemParameters<ListItemDelimiterParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParameters"/> class.
        /// </summary>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="startValue">Start value (0 for decimal numerals).</param>
        /// <param name="valueBase">Value base (10 for decimal numerals).</param>
        /// <param name="maxMarkerLength">Maximum marker length.</param>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="markerType">Marker type.</param>
        /// <param name="listStyle">List style.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public OrderedListItemParameters(char markerMinChar, char markerMaxChar, int startValue = 0, int valueBase = 0, int maxMarkerLength = 9,
            BlockTag tag = BlockTag.ListItem, BlockTag parentTag = BlockTag.OrderedList, ListType listType = ListType.Ordered,
            OrderedListMarkerType markerType = OrderedListMarkerType.None, string listStyle = null, ListItemDelimiterParameters[] delimiters = null)
#pragma warning restore 0618
            : this(new[] { new OrderedListMarkerRangeParameters(markerMinChar, markerMaxChar, startValue) }, valueBase, maxMarkerLength, tag, parentTag, listType, markerType, listStyle, delimiters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedListItemParameters"/> class.
        /// </summary>
        /// <param name="markers">Marker parameters.</param>
        /// <param name="valueBase">Value base (1 for additive lists).</param>
        /// <param name="maxMarkerLength">Maximum marker length.</param>
        /// <param name="tag">List item element tag.</param>
        /// <param name="parentTag">List element tag.</param>
        /// <param name="listType">List type (obsolete).</param>
        /// <param name="markerType">Marker type.</param>
        /// <param name="listStyle">List style.</param>
        /// <param name="delimiters">Delimiter parameters.</param>
#pragma warning disable 0618
        public OrderedListItemParameters(OrderedListMarkerParameters[] markers, int valueBase = 0, int maxMarkerLength = 9,
            BlockTag tag = BlockTag.ListItem, BlockTag parentTag = BlockTag.OrderedList, ListType listType = ListType.Ordered,
            OrderedListMarkerType markerType = OrderedListMarkerType.None, string listStyle = null, ListItemDelimiterParameters[] delimiters = null)
            : base(tag, parentTag, listType, delimiters ?? ListItemDelimiterParameters.Default)
#pragma warning restore 0618
        {
            this.ValueBase = valueBase;
            this.MaxMarkerLength = maxMarkerLength;
            this.MarkerType = markerType;
            this.Markers = markers;
            this.ListStyle = listStyle;
        }

        /// <summary>
        /// Creates a copy of this parameters object.
        /// </summary>
        /// <returns>Parameters object.</returns>
        public OrderedListItemParameters Clone()
        {
            return (OrderedListItemParameters)MemberwiseClone();
        }

        /// <summary>
        /// Gets or sets the list marker parameters.
        /// </summary>
        public OrderedListMarkerParameters[] Markers { get; set; }

        /// <summary>
        /// Gets or sets the maximum marker length.
        /// </summary>
        public int MaxMarkerLength { get; set; }

        /// <summary>
        /// Gets or sets the list marker type.
        /// </summary>
        public OrderedListMarkerType MarkerType { get; set; }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        public string ListStyle { get; set; }

        /// <summary>
        /// Gets or sets the value base.
        /// </summary>
        public int ValueBase { get; set; }
    }
}
