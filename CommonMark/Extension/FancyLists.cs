using CommonMark.Parser;
using CommonMark.Parser.Blocks;
using CommonMark.Syntax;
using System;
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
        /// <param name="features">Fancy lists features.</param>
        /// <param name="numericListStyles">Numeral list styles.</param>
        /// <param name="additiveListStyles">Additive list styles.</param>
        public FancyListsSettings(FancyListsFeatures features, NumericListStyles numericListStyles, AdditiveListStyles additiveListStyles)
        {
            this.Features = features;
            this.NumericListStyles = numericListStyles;
            this.AdditiveListStyles = additiveListStyles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="features">Fancy lists features.</param>
        public FancyListsSettings(FancyListsFeatures features)
            : this(features, NumericListStyles.None, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="numericListStyles">Numeric list styles.</param>
        public FancyListsSettings(NumericListStyles numericListStyles)
            : this(FancyListsFeatures.None, numericListStyles, AdditiveListStyles.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FancyListsSettings"/> structure.
        /// </summary>
        /// <param name="additiveListStyles">Additive list styles.</param>
        public FancyListsSettings(AdditiveListStyles additiveListStyles)
            : this(FancyListsFeatures.None, NumericListStyles.None, additiveListStyles)
        {
        }

        /// <summary>
        /// Gets or sets the fancy lists features.
        /// </summary>
        public FancyListsFeatures Features { get; set; }

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
            AddRomanHandlers(handlers, settings);
            AddLatinHandlers(handlers, settings);
            AddBulletHandlers(handlers, settings);
            AddNumericHandlers(handlers, settings);
            AddAdditiveHandlers(handlers, settings);
            return handlers;
        }

        private void AddRomanHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetRomanParameters, CreateRomanHandlers, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Roman);
        }

        private static IEnumerable<OrderedListItemParameters> GetRomanParameters()
        {
            yield return RomanNumeralListItemHandler.LowerRomanParameters;
            yield return RomanNumeralListItemHandler.UpperRomanParameters;
        }

        private static IEnumerable<IBlockDelimiterHandler> CreateRomanHandlers(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return RomanNumeralListItemHandler.Create(settings, parameters, parameters.Markers);
        }

        private void AddLatinHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetLatinParameters, CreateAlphaHandlers, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Latin);
        }

        private static IEnumerable<OrderedListItemParameters> GetLatinParameters()
        {
            yield return AlphaListItemHandler.LowerLatinParameters;
            yield return AlphaListItemHandler.UpperLatinParameters;
        }

        private static IEnumerable<IBlockDelimiterHandler> CreateAlphaHandlers(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return AlphaListItemHandler.Create(settings, parameters, parameters.Markers);
        }

        private void AddBulletHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetBulletParameters, CreateBulletHandlers, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Unordered);
        }

        private static IEnumerable<BulletListItemParameters> GetBulletParameters()
        {
            var delimiters = GetBulletDelimiters();
            foreach (var delimiter in delimiters)
                yield return new BulletListItemParameters(delimiters: delimiter);
        }

        private static IEnumerable<BulletListItemDelimiterParameters> GetBulletDelimiters()
        {
            yield return (new BulletListItemDelimiterParameters('●', listStyle: "disc"));
            yield return (new BulletListItemDelimiterParameters('○', listStyle: "circle"));
            yield return (new BulletListItemDelimiterParameters('■', listStyle: "square"));
            yield return (new BulletListItemDelimiterParameters('∙', listStyle: "none"));
        }

        private static IEnumerable<IBlockDelimiterHandler> CreateBulletHandlers(CommonMarkSettings settings, BulletListItemParameters parameters)
        {
            return BulletListItemHandler.Create(settings, parameters);
        }

        private void AddNumericHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetNumericParameters, CreateOrderedHandlers, (int)fancyListsSettings.NumericListStyles, (int)NumericListStyles.All);
        }

        private static IEnumerable<OrderedListItemParameters> GetNumericParameters()
        {
            var delimiters = new ListItemDelimiterParameters('.');
            yield return new OrderedListItemParameters(listStyle: "arabic-indic", markerMinChar: '٠', markerMaxChar: '٩', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "persian", markerMinChar: '۰', markerMaxChar: '۹', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "thai", markerMinChar: '๐', markerMaxChar: '๙', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "cambodian", markerMinChar: '០', markerMaxChar: '៩', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "khmer", markerMinChar: '០', markerMaxChar: '៩', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "lao", markerMinChar: '໐', markerMaxChar: '໙', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "myanmar", markerMinChar: '၀', markerMaxChar: '၉', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "shan", markerMinChar: '႐', markerMaxChar: '႙', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "devanagari", markerMinChar: '०', markerMaxChar: '९', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "bengali", markerMinChar: '০', markerMaxChar: '৯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "eastern-nagari", markerMinChar: '০', markerMaxChar: '৯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "gujarati", markerMinChar: '૦', markerMaxChar: '૯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "gurmukhi", markerMinChar: '੦', markerMaxChar: '੯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "kannada", markerMinChar: '೦', markerMaxChar: '೯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "lepcha", markerMinChar: '᱀', markerMaxChar: '᱉', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "malayalam", markerMinChar: '൦', markerMaxChar: '൧', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "marathi", markerMinChar: '०', markerMaxChar: '९', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "oriya", markerMinChar: '୦', markerMaxChar: '୯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "tamil", markerMinChar: '௦', markerMaxChar: '௯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "telugu", markerMinChar: '౦', markerMaxChar: '౯', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "tibetan", markerMinChar: '༠', markerMaxChar: '༩', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "mongolian", markerMinChar: '᠐', markerMaxChar: '᠙', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "cjk-decimal", markerMinChar: '〇', markerMaxChar: '九', delimiters: delimiters);
            yield return new OrderedListItemParameters(listStyle: "fullwidth-decimal", markerMinChar: '０', markerMaxChar: '９', delimiters: delimiters);
        }

        private static IEnumerable<IBlockDelimiterHandler> CreateOrderedHandlers(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return OrderedListItemHandler.Create(settings, parameters);
        }

        private void AddAdditiveHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetAdditiveParameters, CreateOrderedHandlers, (int)fancyListsSettings.AdditiveListStyles, (int)AdditiveListStyles.All);
        }

        private static IEnumerable<OrderedListItemParameters> GetAdditiveParameters()
        {
            yield return GetHebrewParameters();
        }

        private static OrderedListItemParameters GetHebrewParameters()
        {
            return new OrderedListItemParameters(listStyle: "hebrew", markers: new OrderedListMarkerParameters[]
            {
                new OrderedListMarkerRangeParameters('א', 'י', 1, 1),
                new OrderedListSingleMarkerParameters('כ', 20),
                new OrderedListSingleMarkerParameters('ל', 30),
                new OrderedListSingleMarkerParameters('מ', 40),
                new OrderedListSingleMarkerParameters('נ', 50),
                new OrderedListSingleMarkerParameters('ס', 60),
                new OrderedListSingleMarkerParameters('ע', 70),
                new OrderedListSingleMarkerParameters('פ', 80),
                new OrderedListSingleMarkerParameters('צ', 90),
                new OrderedListSingleMarkerParameters('ק', 100),
                new OrderedListSingleMarkerParameters('ר', 200),
                new OrderedListSingleMarkerParameters('ש', 300),
                new OrderedListSingleMarkerParameters('ת', 400),
            },
            maxMarkerLength: 3,
            delimiters: new ListItemDelimiterParameters('.', 2));
        }

        private static List<IBlockDelimiterHandler> AddHandlers<TParameters>(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings, Func<IEnumerable<TParameters>> sourceFactory,
            Func<CommonMarkSettings, TParameters, IEnumerable<IBlockDelimiterHandler>> handlersFactory, int flags, int flagsMask)
        {
            if ((flags & flagsMask) != 0)
            {
                var mask = flagsMask & ~(flagsMask << 1);
                var source = sourceFactory();
                foreach (var item in source)
                {
                    if (0 != (flags & mask))
                        handlers.AddRange(handlersFactory(settings, item));
                    mask <<= 1;
                }
            }
            return handlers;
        }
    }
}
