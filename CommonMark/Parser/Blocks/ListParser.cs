using CommonMark.Syntax;

namespace CommonMark.Parser.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.List"/> element parser.
    /// </summary>
    public class ListParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public ListParser(CommonMarkSettings settings)
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
            return childTag == BlockTag.ListItem;
        }

        /// <summary>
        /// Initializes a handled element.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Initialize(ref BlockParserInfo info)
        {
            return true;
        }

        /// <summary>
        /// Finalizes a handled element.
        /// </summary>
        /// <param name="container">Block element.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Finalize(Block container)
        {
            // determine tight/loose status
            container.ListData.IsTight = true; // tight by default
            var item = container.FirstChild;
            Block subitem;

            while (item != null)
            {
                // check for non-final non-empty list item ending with blank line:
                if (item.IsLastLineBlank && item.NextSibling != null)
                {
                    container.ListData.IsTight = false;
                    break;
                }

                // recurse into children of list item, to see if there are spaces between them:
                subitem = item.FirstChild;
                while (subitem != null)
                {
                    if (EndsWithBlankLine(subitem) && (item.NextSibling != null || subitem.NextSibling != null))
                    {
                        container.ListData.IsTight = false;
                        break;
                    }

                    subitem = subitem.NextSibling;
                }

                if (!container.ListData.IsTight)
                    break;

                item = item.NextSibling;
            }

            return true;
        }

        /// <summary>
        /// Check to see if a block ends with a blank line, descending if needed into lists and sublists.
        /// </summary>
        private static bool EndsWithBlankLine(Block block)
        {
            while (true)
            {
                if (block.IsLastLineBlank)
                    return true;

                if (block.Tag != BlockTag.List && block.Tag != BlockTag.ListItem)
                    return false;

                block = block.LastChild;

                if (block == null)
                    return false;
            }
        }
    }
}
