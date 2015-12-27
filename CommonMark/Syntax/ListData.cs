using System;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for list block elements. Used in <see cref="Block.ListData"/> property.
    /// </summary>
    [Obsolete("This API has been superseded by " + nameof(UnorderedListData) + " and " + nameof(OrderedListData) + ".")]
    public sealed class ListData : ListData<ListData>
    {
        /// <summary>
        /// Gets or sets the number for the first list item if <see cref="ListData.ListType"/> is set to
        /// <see cref="F:ListType.Ordered"/>.
        /// </summary>
        [Obsolete("This API has been superseded by " + nameof(OrderedListData) + ".")]
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the character used for unordered lists. Used if <see cref="ListData.ListType"/> is set to
        /// <see cref="F:ListType.Bullet"/>.
        /// </summary>
        [Obsolete("This API has been superseded by " + nameof(UnorderedListData) + ".")]
        public char BulletChar { get; set; }

        /// <summary>
        /// Gets or sets the type (ordered or unordered) of this list.
        /// </summary>
        [Obsolete("This API has been superseded by " + nameof(BlockTag.UnorderedList) + " and " + nameof(BlockTag.OrderedList) + ".")]
        public ListType ListType { get; set; }

        /// <summary>
        /// Gets or sets the character that follows the number if <see cref="ListData.ListType"/> is set to
        /// <see cref="F:ListType.Ordered"/>.
        /// </summary>
        [Obsolete("This API has been superseded by " + nameof(OrderedListData) + ".")]
        public ListDelimiter Delimiter { get; set; }
    }

    /// <summary>
    /// Base list data class.
    /// </summary>
    /// <typeparam name="T">Type of list data.</typeparam>
    public abstract class ListData<T>
        where T : ListData<T>
    {
        /// <summary>
        /// Gets or sets the number of spaces the list markers are indented.
        /// </summary>
        public int MarkerOffset { get; set; }

        /// <summary>
        /// Gets or sets the position of the list item contents in the source text line.
        /// </summary>
        public int Padding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the list is tight (such list will not render additional explicit
        /// paragraph elements).
        /// </summary>
        public bool IsTight { get; set; }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Determines whether the specified object contains matching list data.
        /// </summary>
        /// <param name="obj">Candidate object.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a matching list data object containing matching data.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as T;
            return other != null
                && ((this.Style == null && other.Style == null) || (this.Style != null && this.Style.Equals(other.Style)));
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode()
                ^ (Style != null ? Style.GetHashCode() : 0);
        }
    }
}
