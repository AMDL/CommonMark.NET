using CommonMark.Syntax;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// HTML entity parser.
    /// </summary>
    public class EntityParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public EntityParser(CommonMarkSettings settings)
            : base(settings, '&')
        {
        }

        /// <summary>
        /// Parses an HTML entity.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        /// <remarks>Original: Inline HandleEntity(Subject subj)</remarks>
        public override Inline Handle(Block container, Subject subj)
        {
            var origPos = subj.Position;
            return new Inline(ParseEntity(subj), origPos, subj.Position);
        }
    }
}
