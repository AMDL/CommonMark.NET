using System;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the delimeter used in the source for ordered lists.
    /// </summary>
    [Obsolete("This API has been superseded by " + nameof(OrderedListData) + ".")]
    public enum ListDelimiter
    {
        /// <summary>
        /// The item numbering is followed with a period (<c>1. foo</c>).
        /// </summary>
        Period = 0,

        /// <summary>
        /// The item numbering is followed with a closing parenthesis (<c>1) foo</c>).
        /// </summary>
        Parenthesis
    }
}
