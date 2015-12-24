using System;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the type of a list block element.
    /// </summary>
    [Obsolete("This API has been superceded by " + nameof(BlockTag.UnorderedList) + " and " + nameof(BlockTag.OrderedList) + ".")]
    public enum ListType
    {
        /// <summary>
        /// The list is unordered and its items are represented with bullets.
        /// </summary>
        [Obsolete("Use " + nameof(BlockTag.UnorderedList) + " instead.")]
        Bullet = 0,

        /// <summary>
        /// The list is ordered and its items are numbered.
        /// </summary>
        [Obsolete("Use " + nameof(BlockTag.OrderedList) + " instead.")]
        Ordered
    }
}
