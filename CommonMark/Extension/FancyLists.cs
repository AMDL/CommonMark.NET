using CommonMark.Parser;
using CommonMark.Parser.Blocks;
using CommonMark.Parser.Blocks.Delimiters;
using System;
using System.Collections.Generic;

namespace CommonMark.Extension
{
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
            AddSharpHandlers(handlers, settings);
            AddUnorderedHandlers(handlers, settings);
            AddNumericHandlers(handlers, settings);
            AddAdditiveHandlers(handlers, settings);
            return handlers;
        }

        private void AddRomanHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetRomanParameters, CreateRomanHandler, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Roman);
        }

        private static IEnumerable<OrderedListItemParameters> GetRomanParameters()
        {
            yield return RomanListItemHandler.LowerRomanParameters;
            yield return RomanListItemHandler.UpperRomanParameters;
        }

        private static IBlockDelimiterHandler CreateRomanHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return new RomanListItemHandler(settings, parameters);
        }

        private void AddLatinHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetLatinParameters, CreateLatinHandler, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Latin);
        }

        private static IEnumerable<OrderedListItemParameters> GetLatinParameters()
        {
            yield return LatinListItemHandler.LowerLatinParameters;
            yield return LatinListItemHandler.UpperLatinParameters;
        }

        private static IBlockDelimiterHandler CreateLatinHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return new LatinListItemHandler(settings, parameters);
        }

        private void AddSharpHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetSharpParameters, CreateSharpHandler, (int)fancyListsSettings.Features, (int)FancyListsFeatures.DecimalSharps);
        }

        private static IEnumerable<OrderedListItemParameters> GetSharpParameters()
        {
            yield return SharpListItemHandler.DefaultParameters;
        }

        private static SharpListItemHandler CreateSharpHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return new SharpListItemHandler(settings, parameters);
        }

        private void AddUnorderedHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetUnorderedParameters, CreateUnorderedHandler, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Unordered);
        }

        private static IEnumerable<UnorderedListItemParameters> GetUnorderedParameters()
        {
            var delimiters = GetUnorderedDelimiters();
            foreach (var delimiter in delimiters)
                yield return new UnorderedListItemParameters(delimiters: delimiter);
        }

        private static IEnumerable<UnorderedListItemDelimiterParameters> GetUnorderedDelimiters()
        {
            yield return (new UnorderedListItemDelimiterParameters('●', listStyle: "disc"));
            yield return (new UnorderedListItemDelimiterParameters('○', listStyle: "circle"));
            yield return (new UnorderedListItemDelimiterParameters('■', listStyle: "square"));
            yield return (new UnorderedListItemDelimiterParameters('∙', listStyle: "none"));
        }

        private static IBlockDelimiterHandler CreateUnorderedHandler(CommonMarkSettings settings, UnorderedListItemParameters parameters)
        {
            return new UnorderedListItemHandler(settings, parameters, parameters.Delimiters[0]);
        }

        private void AddNumericHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetNumericParameters, CreateNumericHandler, (int)fancyListsSettings.NumericListStyles, (int)NumericListStyles.All);
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

        private static IBlockDelimiterHandler CreateNumericHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return new NumericListItemHandler(settings, parameters);
        }

        private void AddAdditiveHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetAdditiveParameters, CreateAlphaHandler, (int)fancyListsSettings.AdditiveListStyles, (int)AdditiveListStyles.All);
        }

        private static IEnumerable<OrderedListItemParameters> GetAdditiveParameters()
        {
            yield return GetHebrewParameters();
        }

        private static OrderedListItemParameters GetHebrewParameters()
        {
            return new OrderedListItemParameters(listStyle: "hebrew", markers: new OrderedListMarkerParameters[]
            {
                new OrderedListMarkerRangeParameters('א', 'י', 1),
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

        private static IBlockDelimiterHandler CreateAlphaHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            return new AlphaListItemHandler(settings, parameters);
        }

        private static List<IBlockDelimiterHandler> AddHandlers<TParameters>(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings, Func<IEnumerable<TParameters>> sourceFactory,
            Func<CommonMarkSettings, TParameters, IBlockDelimiterHandler> handlerFactory, int flags, int flagsMask)
        {
            if ((flags & flagsMask) != 0)
            {
                var mask = flagsMask & ~(flagsMask << 1);
                var source = sourceFactory();
                foreach (var item in source)
                {
                    if (0 != (flags & mask))
                        handlers.Add(handlerFactory(settings, item));
                    mask <<= 1;
                }
            }
            return handlers;
        }
    }
}
