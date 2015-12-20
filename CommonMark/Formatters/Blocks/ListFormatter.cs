﻿using CommonMark.Syntax;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// Base list formatter class.
    /// </summary>
    /// <typeparam name="TData">Type of list data.</typeparam>
    public abstract class ListFormatter<TData> : BlockFormatter
        where TData : ListData<TData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFormatter{TData}"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="htmlTag">HTML tag.</param>
        /// <param name="printerTag">Printer tag.</param>
        public ListFormatter(FormatterParameters parameters, BlockTag tag, string htmlTag, string printerTag)
            : base(parameters, tag, htmlTag, printerTag)
        {
        }

        /// <summary>
        /// Writes the start of a list element opening.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">List element.</param>
        protected void StartWriteOpening(IHtmlTextWriter writer, Block element)
        {
            writer.EnsureLine();
            writer.Write('<');
            writer.WriteConstant(HtmlTag);
        }

        /// <summary>
        /// Writes the end of a list element opening.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">List element.</param>
        /// <param name="listData">Specific list data.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        protected bool CompleteWriteOpening(IHtmlTextWriter writer, Block element, TData listData)
        {
            if (listData.ListStyle != null)
            {
                writer.WriteConstant(" style=\"list-style-type: ");
                writer.WriteConstant(listData.ListStyle);
                writer.Write('\"');
            }
            WritePosition(writer, element);
            writer.WriteLine('>');
            return true;
        }

        /// <summary>
        /// Returns the paragraph stacking option for a block element.
        /// </summary>
        /// <param name="element">Block element.</param>
        /// <param name="tight">The parent's stacking option.</param>
        /// <returns>
        /// <c>true</c> to stack paragraphs tightly,
        /// <c>false</c> to stack paragraphs loosely,
        /// or <c>null</c> to skip paragraph stacking.
        /// </returns>
        public override bool? IsStackTight(Block element, bool tight)
        {
            return element.ListData.IsTight;
        }
    }
}
