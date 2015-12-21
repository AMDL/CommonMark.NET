﻿using System.Collections.Generic;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for list block elements. Used in <see cref="Block.DocumentData"/> property.
    /// </summary>
    public class DocumentData
    {
        /// <summary>
        /// Gets or sets the dictionary containing resolved link references. Only set on the document node, <c>null</c>
        /// and not used for all other elements.
        /// </summary>
        public Dictionary<string, Reference> ReferenceMap { get; set; }
    }
}