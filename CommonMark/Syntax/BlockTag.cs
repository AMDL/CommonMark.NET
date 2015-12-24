using System;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Specifies the element type of a <see cref="Block"/> instance.
    /// </summary>
    public enum BlockTag : byte
    {
        /// <summary>
        /// The root element that represents the document itself. There should only be one in the tree.
        /// </summary>
        Document,

        /// <summary>
        /// A block-quote element.
        /// </summary>
        BlockQuote,

        /// <summary>
        /// A list element. Will contain nested blocks with type of <see cref="BlockTag.ListItem"/>.
        /// </summary>
        [Obsolete("This API has been superceded by " + nameof(BulletList) + " and " + nameof(OrderedList) + ".")]
        List,

        /// <summary>
        /// An item in a block element of type <see cref="BlockTag.List"/>.
        /// </summary>
        ListItem,

        /// <summary>
        /// A code block element that was formatted with fences (for example, <c>~~~\nfoo\n~~~</c>).
        /// </summary>
        FencedCode,

        /// <summary>
        /// A code block element that was formatted by indenting the lines with at least 4 spaces.
        /// </summary>
        IndentedCode,

        /// <summary>
        /// A raw HTML code block element.
        /// </summary>
        HtmlBlock,

        /// <summary>
        /// A paragraph block element.
        /// </summary>
        Paragraph,

        /// <summary>
        /// A heading element that was parsed from an ATX style markup (<c>## heading 2</c>).
        /// </summary>
        AtxHeading,

        /// <summary>
        /// Obsolete. Use <see cref="AtxHeading"/> instead.
        /// </summary>
        [Obsolete("Use " + nameof(AtxHeading) + " instead.")]
        AtxHeader = AtxHeading,

        /// <summary>
        /// A heading element that was parsed from a Setext style markup (<c>heading\n========</c>).
        /// </summary>
        SETextHeading,

        /// <summary>
        /// Obsolete. Use <see cref="SETextHeading"/> instead.
        /// </summary>
        [Obsolete("Use " + nameof(SETextHeading) + " instead.")]
        SETextHeader = SETextHeading,

        /// <summary>
        /// A thematic break element.
        /// </summary>
        ThematicBreak,

        /// <summary>
        /// Obsolete. Use <see cref="ThematicBreak"/> instead.
        /// </summary>
        [Obsolete("Use " + nameof(ThematicBreak) + " instead.")]
        HorizontalRuler = ThematicBreak,

        /// <summary>
        /// A text block that contains only link reference definitions.
        /// </summary>
        ReferenceDefinition,

        /// <summary>
        /// A bulleted list element. Will contain nested blocks with type of <see cref="BlockTag.ListItem"/>.
        /// </summary>
        BulletList,

        /// <summary>
        /// An ordered list element. Will contain nested blocks with type of <see cref="BlockTag.ListItem"/>.
        /// </summary>
        OrderedList,

        /// <summary>
        /// A definition list element. Will contain <see cref="Term"/> and <see cref="Definition"/> blocks.
        /// Only present if <see cref="Extension.DefinitionLists"/> are enabled.
        /// </summary>
        DefinitionList,

        /// <summary>
        /// A defined term in a <see cref="DefinitionList"/> block.
        /// Only present if <see cref="Extension.DefinitionLists"/> are enabled.
        /// </summary>
        Term,

        /// <summary>
        /// A term definition in a <see cref="DefinitionList"/> block.
        /// Only present if <see cref="Extension.DefinitionLists"/> are enabled.
        /// </summary>
        Definition,

        /// <summary>
        /// A table element.
        /// Will contain an optional <see cref="TableCaption"/> element,
        /// zero or more <see cref="TableColumnGroup"/> elements,
        /// an optional <see cref="TableHeader"/> element,
        /// an optional <see cref="TableFooter"/> element,
        /// and zero or more <see cref="TableBody"/> elements.
        /// Only present if tables are enabled.
        /// </summary>
        Table,

        /// <summary>
        /// A table caption element.
        /// Only present if both tables and <see cref="Extension.TableCaptions"/> are enabled.
        /// </summary>
        TableCaption,

        /// <summary>
        /// A table column group element. Will contain <see cref="TableColumn"/> elements.
        /// Only present if both tables and table column groups are enabled.
        /// </summary>
        TableColumnGroup,

        /// <summary>
        /// A table column element.
        /// Only present if both tables and table column groups are enabled.
        /// </summary>
        TableColumn,

        /// <summary>
        /// A table header element. Will contain <see cref="TableRow"/> elements.
        /// Only present if tables are enabled.
        /// </summary>
        TableHeader,

        /// <summary>
        /// A table footer element. Will contain <see cref="TableRow"/> elements.
        /// Only present if both tables and table footers are enabled.
        /// </summary>
        TableFooter,

        /// <summary>
        /// A table body element. Will contain <see cref="TableRow"/> elements.
        /// Only present if tables are enabled.
        /// </summary>
        TableBody,

        /// <summary>
        /// A table row element. Will contain <see cref="TableCell"/> elements.
        /// Only present if tables are enabled.
        /// </summary>
        TableRow,

        /// <summary>
        /// A table cell element.
        /// Only present if tables are enabled.
        /// </summary>
        TableCell,

        /// <summary>
        /// A figure element.
        /// Only present if <see cref="Extension.Figures"/> are enabled.
        /// </summary>
        Figure,

        /// <summary>
        /// A figure caption element.
        /// Only present in a <see cref="Figure"/> block if <see cref="Extension.Figures"/> are enabled.
        /// </summary>
        FigureCaption,

        /// <summary>
        /// Custom block element.
        /// Any additional properties are set through <see cref="Block.CustomData"/>.
        /// </summary>
        Custom,

        /// <summary>
        /// Block tag count. There should be no elements with this tag.
        /// </summary>
        Count
    }
}
