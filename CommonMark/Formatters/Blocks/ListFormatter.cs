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
        /// <param name="textTag">Text syntax tree tag.</param>
        /// <param name="htmlTags">HTML tag.</param>
        public ListFormatter(FormatterParameters parameters, BlockTag tag, string textTag, params string[] htmlTags)
            : base(parameters, tag, textTag, htmlTags)
        {
            IsList = true;
        }

        /// <summary>
        /// Writes the end of a list element opening.
        /// </summary>
        /// <param name="writer">HTML writer.</param>
        /// <param name="element">List element.</param>
        /// <param name="list">Specific list data.</param>
        /// <returns><c>true</c> if the parent formatter should visit the child elements.</returns>
        protected bool CompleteWriteOpening(IHtmlTextWriter writer, Block element, TData list)
        {
            if (list.Style != null && Parameters.IsOutputListStyles)
            {
                writer.WriteConstant(" style=\"list-style-type: ");
                writer.WriteConstant(list.Style);
                writer.Write('\"');
            }
            writer.WriteLine('>');
            return true;
        }
    }
}
