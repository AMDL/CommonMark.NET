using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the type of a list block element.
    /// </summary>
    [Obsolete("This API has been superceded by " + nameof(BlockTag.BulletList) + " and " + nameof(BlockTag.OrderedList) + ".")]
    public enum ListType
    {
        /// <summary>
        /// The list is unordered and its items are represented with bullets.
        /// </summary>
        [Obsolete("Use " + nameof(BlockTag.BulletList) + " instead.")]
        Bullet = 0,

        /// <summary>
        /// The list is ordered and its items are numbered.
        /// </summary>
        [Obsolete("Use " + nameof(BlockTag.OrderedList) + " instead.")]
        Ordered
    }
}
