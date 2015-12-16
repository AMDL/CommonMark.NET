using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.Document"/> element parser.
    /// </summary>
    public class DocumentParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public DocumentParser(CommonMarkSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Determines whether a handled element can contain child elements of the specified kind.
        /// </summary>
        /// <param name="childTag">Block element tag.</param>
        /// <returns><c>true</c> if handled elements can contain elements having <paramref name="childTag"/>.</returns>
        public override bool CanContain(BlockTag childTag)
        {
            return true;
        }
    }
}
