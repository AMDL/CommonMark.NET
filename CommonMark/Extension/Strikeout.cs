using CommonMark.Formatters;
using CommonMark.Formatters.Inlines;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Double tildes will be interpreted as strikeout markers.
    /// </summary>
    public class Strikeout : CommonMarkExtension
    {
        private readonly StrikethroughFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Strikeout"/> class.
        /// </summary>
        /// <param name="settings">The object containing settings to be cloned for the formatting process.</param>
        public Strikeout(CommonMarkSettings settings)
        {
            this.formatter = new StrikethroughFormatter(settings.Clone());
        }

        /// <summary>
        /// Gets the mapping from character to inline tag for matched double-character openers.
        /// </summary>
        public override IDictionary<char, InlineTag> DoubleCharTags
        {
            get
            {
                return new Dictionary<char, InlineTag>
                {
                    { '~', InlineTag.Strikethrough }
                };
            }
        }

        /// <summary>
        /// Gets the mapping from inline tag to inline element formatter.
        /// </summary>
        public override IDictionary<InlineTag, IInlineFormatter> InlineFormatters
        {
            get
            {
                return new Dictionary<InlineTag, IInlineFormatter>
                {
                    { InlineTag.Strikethrough, formatter }
                };
            }
        }

        /// <summary>
        /// Determines whether the specified object is <see cref="Strikeout"/>.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if the object is a <see cref="Strikeout"/> instance.</returns>
        public override bool Equals(object obj)
        {
            return obj is Strikeout;
        }

        /// <summary>
        /// Returns the hash code of the type object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
