﻿namespace CommonMark.Syntax
{
    /// <summary>
    /// Specifies the element type of an <see cref="Inline"/> instance.
    /// </summary>
    public enum InlineTag : byte
    {
        /// <summary>
        /// Represents a simple literal string content. Uses <see cref="Inline.LiteralContent"/> to specify the data.
        /// Cannot contain nested elements.
        /// </summary>
        String = 0,

        /// <summary>
        /// Represents a soft-break which by default is rendered as a simple newline and thus does not impact
        /// the display of the resulting HTML code. The <see cref="CommonMarkSettings.RenderSoftLineBreaksAsLineBreaks"/>
        /// property can be used to override this behavior and render soft-breaks as <c>&lt;br;&gt;</c> HTML
        /// elements.
        /// Cannot contain literal content or nested elements.
        /// </summary>
        SoftBreak,

        /// <summary>
        /// Represents a line-break which by default is rendered as a <c>&lt;br;&gt;</c> HTML element.
        /// Cannot contain literal content or nested elements.
        /// </summary>
        LineBreak,

        /// <summary>
        /// Represents an inline code element. Uses <see cref="Inline.LiteralContent"/> to specify the data.
        /// Cannot contain nested elements.
        /// </summary>
        Code,

        /// <summary>
        /// Represents raw HTML code. Uses <see cref="Inline.LiteralContent"/> to specify the data.
        /// Cannot contain nested elements.
        /// </summary>
        RawHtml,

        /// <summary>
        /// Represents an emphasis element. Uses <see cref="Inline.FirstChild"/> to specify the contents.
        /// Cannot contain literal content.
        /// </summary>
        Emphasis,

        /// <summary>
        /// Represents a strong emphasis element. Uses <see cref="Inline.FirstChild"/> to specify the contents.
        /// Cannot contain literal content.
        /// </summary>
        Strong,

        /// <summary>
        /// Represents a link element. Uses <see cref="Inline.FirstChild"/> to specify the content (or label).
        /// Uses <see cref="Inline.TargetUrl"/> to specify the target of the link and 
        /// <see cref="Inline.LiteralContent"/> to store the title of the link.
        /// </summary>
        Link,

        /// <summary>
        /// Represents an image element. Uses <see cref="Inline.FirstChild"/> to specify the label (description).
        /// Uses <see cref="Inline.TargetUrl"/> to specify the source of the image and 
        /// <see cref="Inline.LiteralContent"/> to store the title of the image.
        /// </summary>
        Image,

        /// <summary>
        /// Represents an inline element that has been "removed" (visually represented as strikethrough).
        /// Only present if <see cref="Extension.Strikeout"/> is registered.
        /// </summary>
        Strikethrough,

        /// <summary>
        /// Represents a subscript element.
        /// Only present if <see cref="Extension.Subscript"/> is registered.
        /// </summary>
        Subscript,

        /// <summary>
        /// Represents a superscript element.
        /// Only present if <see cref="Extension.Superscript"/> is registered.
        /// </summary>
        Superscript,

        /// <summary>
        /// Represents a math element.
        /// Only present if <see cref="Extension.MathDollars"/> is registered.
        /// </summary>
        Math,

        /// <summary>
        /// Represents a custom inline element.
        /// Any additional properties are set through the <see cref="Inline.Custom"/> property.
        /// </summary>
        Custom,

        /// <summary>
        /// Inline tag count. There should be no elements with this tag.
        /// </summary>
        Count
    }
}
