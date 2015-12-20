using CommonMark.Parser;
using CommonMark.Parser.Blocks;
using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Extension
{
    /// <summary>
    /// Fancy lists settings.
    /// </summary>
    public struct FancyListsSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="standardListStyles">Standard list styles.</param>
        /// <param name="bulletListStyles">Bullet list styles.</param>
        /// <param name="numericListStyles">Numeral list styles.</param>
        /// <param name="additiveListStyles">Additive list styles.</param>
        public FancyListsSettings(StandardListStyles standardListStyles, BulletListStyles bulletListStyles, NumericListStyles numericListStyles, AdditiveListStyles additiveListStyles)
        {
            this.StandardListStyles = standardListStyles;
            this.BulletListStyles = bulletListStyles;
            this.NumericListStyles = numericListStyles;
            this.AdditiveListStyles = additiveListStyles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="standardListStyles">Standard list styles.</param>
        public FancyListsSettings(StandardListStyles standardListStyles)
            : this(standardListStyles, BulletListStyles.None, NumericListStyles.None, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="bulletListStyles">Bullet list styles.</param>
        public FancyListsSettings(BulletListStyles bulletListStyles)
            : this(StandardListStyles.None, bulletListStyles, NumericListStyles.None, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="numericListStyles">Numeric list styles.</param>
        public FancyListsSettings(NumericListStyles numericListStyles)
            : this(StandardListStyles.None, BulletListStyles.None, numericListStyles, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="additiveListStyles">Additive list styles.</param>
        public FancyListsSettings(AdditiveListStyles additiveListStyles)
            : this(StandardListStyles.None, BulletListStyles.None, NumericListStyles.None, additiveListStyles)
        {
        }

        /// <summary>
        /// Gets or sets the standard list styles.
        /// </summary>
        public StandardListStyles StandardListStyles { get; set; }

        /// <summary>
        /// Gets or sets the =bullet list styles.
        /// </summary>
        public BulletListStyles BulletListStyles { get; set; }

        /// <summary>
        /// Gets or sets the numeric list styles.
        /// </summary>
        public NumericListStyles NumericListStyles { get; set; }

        /// <summary>
        /// Gets or sets the additive list styles.
        /// </summary>
        public AdditiveListStyles AdditiveListStyles { get; set; }
    }

    /// <summary>
    /// Fancy lists.
    /// </summary>
    public class FancyLists : CommonMarkExtension
    {
        private readonly FancyListsSettings fancyListsSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyLists"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="fancyListsSettings">Fancy lists settings.</param>
        public FancyLists(CommonMarkSettings settings, FancyListsSettings fancyListsSettings)
            : base(settings)
        {
            this.fancyListsSettings = fancyListsSettings;
        }

        /// <summary>
        /// Initializes the block delimiter handlers.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <returns>Block delimiter handlers.</returns>
        protected override IEnumerable<IBlockDelimiterHandler> InitializeBlockDelimiterHandlers(CommonMarkSettings settings)
        {
            var handlers = new List<IBlockDelimiterHandler>();

            var bulletDelimiters = new List<BulletListItemDelimiterParameters>(GetBulletDelimiters());
            var bulletParameters = new BulletListItemParameters(delimiters: bulletDelimiters.ToArray());
            handlers.AddRange(BulletListItemHandler.Create(settings, bulletParameters));

            var orderedParameters = new List<OrderedListItemParameters>(GetStandardParameters());
            orderedParameters.AddRange(GetNumericParameters());
            orderedParameters.AddRange(GetAdditiveParameters());
            foreach (var item in orderedParameters)
                handlers.AddRange(OrderedListItemHandler.Create(settings, item));

            return handlers;
        }

        private IEnumerable<BulletListItemDelimiterParameters> GetBulletDelimiters()
        {
            if (IsEnabled(BulletListStyles.Disc))
                yield return new BulletListItemDelimiterParameters('●', listStyle: "disc");
            if (IsEnabled(BulletListStyles.Circle))
                yield return new BulletListItemDelimiterParameters('○', listStyle: "circle");
            if (IsEnabled(BulletListStyles.Square))
                yield return new BulletListItemDelimiterParameters('■', listStyle: "square");
        }

        private IEnumerable<OrderedListItemParameters> GetStandardParameters()
        {
            var lowerDelimiters = new[]
                {
                    new ListItemDelimiterParameters('.', 1),
                    new ListItemDelimiterParameters(')', 1),
                };
            
            if (IsEnabled(StandardListStyles.LowerRoman))
            {
                yield return new OrderedListItemParameters(markerType: OrderedListMarkerType.LowerRoman, listStyle: "lower-roman", markerMinChar: 'i', markerMaxChar: 'i', maxMarkerLength: 5, delimiters: lowerDelimiters);
                yield return new OrderedListItemParameters(markerType: OrderedListMarkerType.LowerRoman, listStyle: "lower-roman", markerMinChar: 'v', markerMaxChar: 'v', maxMarkerLength: 5, delimiters: lowerDelimiters);
            }

            if (IsEnabled(StandardListStyles.LowerLatin))
            {
                yield return new OrderedListItemParameters(markerType: OrderedListMarkerType.LowerLatin, listStyle: "lower-latin", markerMinChar: 'a', markerMaxChar: 'z', maxMarkerLength: 3, delimiters: lowerDelimiters);
            }

            var upperDelimiters = new[]
                {
                    new ListItemDelimiterParameters('.', 2),
                    new ListItemDelimiterParameters(')', 1),
                };

            if (IsEnabled(StandardListStyles.UpperRoman))
            {
                yield return new OrderedListItemParameters(markerType: OrderedListMarkerType.UpperRoman, listStyle: "upper-roman", markerMinChar: 'I', markerMaxChar: 'I', maxMarkerLength: 5, delimiters: lowerDelimiters);
                yield return new OrderedListItemParameters(markerType: OrderedListMarkerType.UpperRoman, listStyle: "upper-roman", markerMinChar: 'V', markerMaxChar: 'V', maxMarkerLength: 5, delimiters: lowerDelimiters);
            }

            if (IsEnabled(StandardListStyles.UpperLatin))
            {
                yield return new OrderedListItemParameters(markerType: OrderedListMarkerType.UpperLatin, listStyle: "upper-latin", markerMinChar: 'A', markerMaxChar: 'Z', maxMarkerLength: 3, delimiters: upperDelimiters);
            }
        }

        private IEnumerable<OrderedListItemParameters> GetNumericParameters()
        {
            var delimiters = new ListItemDelimiterParameters('.');
            if (IsEnabled(NumericListStyles.ArabicIndic))
                yield return new OrderedListItemParameters(listStyle: "arabic-indic", markerMinChar: '٠', markerMaxChar: '٩', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Persian))
                yield return new OrderedListItemParameters(listStyle: "persian", markerMinChar: '۰', markerMaxChar: '۹', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Thai))
                yield return new OrderedListItemParameters(listStyle: "thai", markerMinChar: '๐', markerMaxChar: '๙', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Cambodian))
                yield return new OrderedListItemParameters(listStyle: "cambodian", markerMinChar: '០', markerMaxChar: '៩', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Khmer))
                yield return new OrderedListItemParameters(listStyle: "khmer", markerMinChar: '០', markerMaxChar: '៩', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Lao))
                yield return new OrderedListItemParameters(listStyle: "lao", markerMinChar: '໐', markerMaxChar: '໙', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Myanmar))
                yield return new OrderedListItemParameters(listStyle: "myanmar", markerMinChar: '၀', markerMaxChar: '၉', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Shan))
                yield return new OrderedListItemParameters(listStyle: "shan", markerMinChar: '႐', markerMaxChar: '႙', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Devanagari))
                yield return new OrderedListItemParameters(listStyle: "devanagari", markerMinChar: '०', markerMaxChar: '९', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Bengali))
                yield return new OrderedListItemParameters(listStyle: "bengali", markerMinChar: '০', markerMaxChar: '৯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.EasternNagari))
                yield return new OrderedListItemParameters(listStyle: "eastern-nagari", markerMinChar: '০', markerMaxChar: '৯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Gujarati))
                yield return new OrderedListItemParameters(listStyle: "gujarati", markerMinChar: '૦', markerMaxChar: '૯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Gurmukhi))
                yield return new OrderedListItemParameters(listStyle: "gurmukhi", markerMinChar: '੦', markerMaxChar: '੯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Kannada))
                yield return new OrderedListItemParameters(listStyle: "kannada", markerMinChar: '೦', markerMaxChar: '೯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Lepcha))
                yield return new OrderedListItemParameters(listStyle: "lepcha", markerMinChar: '᱀', markerMaxChar: '᱉', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Malayalam))
                yield return new OrderedListItemParameters(listStyle: "malayalam", markerMinChar: '൦', markerMaxChar: '൧', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Marathi))
                yield return new OrderedListItemParameters(listStyle: "marathi", markerMinChar: '०', markerMaxChar: '९', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Oriya))
                yield return new OrderedListItemParameters(listStyle: "oriya", markerMinChar: '୦', markerMaxChar: '୯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Tamil))
                yield return new OrderedListItemParameters(listStyle: "tamil", markerMinChar: '௦', markerMaxChar: '௯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Telugu))
                yield return new OrderedListItemParameters(listStyle: "telugu", markerMinChar: '౦', markerMaxChar: '౯', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Tibetan))
                yield return new OrderedListItemParameters(listStyle: "tibetan", markerMinChar: '༠', markerMaxChar: '༩', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.Mongolian))
                yield return new OrderedListItemParameters(listStyle: "mongolian", markerMinChar: '᠐', markerMaxChar: '᠙', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.CJKDecimal))
                yield return new OrderedListItemParameters(listStyle: "cjk-decimal", markerMinChar: '〇', markerMaxChar: '九', delimiters: delimiters);
            if (IsEnabled(NumericListStyles.FullwidthDecimal))
                yield return new OrderedListItemParameters(listStyle: "fullwidth-decimal", markerMinChar: '０', markerMaxChar: '９', delimiters: delimiters);
        }

        private IEnumerable<OrderedListItemParameters> GetAdditiveParameters()
        {
            var delimiters = new ListItemDelimiterParameters('.', 2);
            if (IsEnabled(AdditiveListStyles.Hebrew))
                yield return new OrderedListItemParameters(listStyle: "hebrew", markerMinChar: 'א', markerMaxChar: 'ת', maxMarkerLength: 3, delimiters: delimiters);
        }

        private bool IsEnabled(StandardListStyles flag)
        {
            return 0 != (fancyListsSettings.StandardListStyles & flag);
        }

        private bool IsEnabled(BulletListStyles flag)
        {
            return 0 != (fancyListsSettings.BulletListStyles & flag);
        }

        private bool IsEnabled(NumericListStyles flag)
        {
            return 0 != (fancyListsSettings.NumericListStyles & flag);
        }

        private bool IsEnabled(AdditiveListStyles flag)
        {
            return 0 != (fancyListsSettings.AdditiveListStyles & flag);
        }
    }
}
