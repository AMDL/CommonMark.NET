using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Formatters.Blocks
{
    /// <summary>
    /// <see cref="BlockTag.BulletList"/> element formatter.
    /// </summary>
    public class BulletListFormatter : ListFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletListFormatter"/> class.
        /// </summary>
        /// <param name="parameters">Formatter parameters.</param>
        /// <param name="tag">Block element tag.</param>
        /// <param name="printerTag">Printer tag.</param>
        public BulletListFormatter(FormatterParameters parameters, BlockTag tag = BlockTag.BulletList, string printerTag = "bullet_list")
            : base(parameters, tag, "ul", printerTag)
        {
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { "tight", element.ListData.IsTight },
                { "bullet_char", element.BulletListData.BulletCharacter },
            };
        }
    }

    internal sealed class LegacyBulletListFormatter : BulletListFormatter
    {
        public LegacyBulletListFormatter(FormatterParameters parameters)
#pragma warning disable 0618
            : base(parameters, BlockTag.List, "list")
#pragma warning restore 0618
        {
        }

        public override bool CanHandle(Block element)
        {
#pragma warning disable 0618
            return element.Tag == BlockTag.List && element.ListData.ListType == ListType.Bullet;
#pragma warning restore 0618
        }

        /// <summary>
        /// Returns the properties of an element.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <param name="element">Element.</param>
        /// <returns>Properties or <c>null</c>.</returns>
        public override IDictionary<string, object> GetPrinterData(IPrinter printer, Block element)
        {
            return new Dictionary<string, object>
            {
                { "type", "bullet" },
                { "tight", element.ListData.IsTight },
#pragma warning disable 0618
                { "bullet_char", element.ListData.BulletChar },
#pragma warning restore 0618
            };
        }
    }
}
