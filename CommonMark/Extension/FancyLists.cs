﻿using CommonMark.Parser;
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
            AddHashHandlers(handlers, settings);
            AddUnorderedHandlers(handlers, settings);
            AddDecimalHandlers(handlers, settings);
            AddNumericHandlers(handlers, settings);
            AddAlphaHandlers(handlers, settings);
            AddAdditiveHandlers(handlers, settings);
            return handlers;
        }

        /// <summary>
        /// Initializes the escapable characters.
        /// </summary>
        protected override IEnumerable<char> InitializeEscapableCharacters()
        {
            return GetCharacters(GetUnorderedParameters, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Unordered);
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

        private static IBlockDelimiterHandler CreateRomanHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            return new RomanListItemHandler(settings, parameters, delimiter);
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

        private static IBlockDelimiterHandler CreateLatinHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            return new LatinListItemHandler(settings, parameters, delimiter);
        }

        private void AddHashHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetHashParameters, CreateHashHandler, (int)fancyListsSettings.Features, (int)FancyListsFeatures.OrderedHashes);
        }

        private static IEnumerable<OrderedListItemParameters> GetHashParameters()
        {
            yield return HashListItemHandler.DefaultParameters;
        }

        private static HashListItemHandler CreateHashHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            return new HashListItemHandler(settings, parameters, delimiter);
        }

        private void AddUnorderedHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetUnorderedParameters, CreateUnorderedHandler, (int)fancyListsSettings.Features, (int)FancyListsFeatures.Unordered);
        }

        private static IEnumerable<UnorderedListItemParameters> GetUnorderedParameters()
        {
            yield return new UnorderedListItemParameters(markerChars: new[] { '•', 'o', '' });
            yield return new UnorderedListItemParameters("disc", '●');
            yield return new UnorderedListItemParameters("circle", '◦');
            yield return new UnorderedListItemParameters("square", '▪');
            yield return new UnorderedListItemParameters("none", '∙');
        }

        private static IBlockDelimiterHandler CreateUnorderedHandler(CommonMarkSettings settings, UnorderedListItemParameters parameters, UnorderedListItemDelimiterParameters delimiter)
        {
            return new UnorderedListItemHandler(settings, parameters, delimiter);
        }

        private void AddDecimalHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            if (0 != (fancyListsSettings.Features & FancyListsFeatures.OrderedHashes))
            {
                var parameters = NumericListItemHandler.DefaultParameters.Clone();
                parameters.MarkerType = Syntax.OrderedListMarkerType.Decimal;
                handlers.Add(NumericListItemHandler.Create(settings, parameters));
            }
        }

        private void AddNumericHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetNumericParameters, CreateNumericHandler, (int)fancyListsSettings.NumericListStyles, (int)NumericListStyles.All);
        }

        private static IEnumerable<OrderedListItemParameters> GetNumericParameters()
        {
            yield return new OrderedListItemParameters(listStyle: "arabic-indic", markerMinChar: '٠', markerMaxChar: '٩');
            yield return new OrderedListItemParameters(listStyle: "persian", markerMinChar: '۰', markerMaxChar: '۹');
            yield return new OrderedListItemParameters(listStyle: "thai", markerMinChar: '๐', markerMaxChar: '๙');
            yield return new OrderedListItemParameters(listStyle: "cambodian", markerMinChar: '០', markerMaxChar: '៩');
            yield return new OrderedListItemParameters(listStyle: "khmer", markerMinChar: '០', markerMaxChar: '៩');
            yield return new OrderedListItemParameters(listStyle: "lao", markerMinChar: '໐', markerMaxChar: '໙');
            yield return new OrderedListItemParameters(listStyle: "myanmar", markerMinChar: '၀', markerMaxChar: '၉');
            yield return new OrderedListItemParameters(listStyle: "shan", markerMinChar: '႐', markerMaxChar: '႙');
            yield return new OrderedListItemParameters(listStyle: "devanagari", markerMinChar: '०', markerMaxChar: '९');
            yield return new OrderedListItemParameters(listStyle: "bengali", markerMinChar: '০', markerMaxChar: '৯');
            yield return new OrderedListItemParameters(listStyle: "eastern-nagari", markerMinChar: '০', markerMaxChar: '৯');
            yield return new OrderedListItemParameters(listStyle: "gujarati", markerMinChar: '૦', markerMaxChar: '૯');
            yield return new OrderedListItemParameters(listStyle: "gurmukhi", markerMinChar: '੦', markerMaxChar: '੯');
            yield return new OrderedListItemParameters(listStyle: "kannada", markerMinChar: '೦', markerMaxChar: '೯');
            yield return new OrderedListItemParameters(listStyle: "lepcha", markerMinChar: '᱀', markerMaxChar: '᱉');
            yield return new OrderedListItemParameters(listStyle: "malayalam", markerMinChar: '൦', markerMaxChar: '൧');
            yield return new OrderedListItemParameters(listStyle: "marathi", markerMinChar: '०', markerMaxChar: '९');
            yield return new OrderedListItemParameters(listStyle: "oriya", markerMinChar: '୦', markerMaxChar: '୯');
            yield return new OrderedListItemParameters(listStyle: "tamil", markerMinChar: '௦', markerMaxChar: '௯');
            yield return new OrderedListItemParameters(listStyle: "telugu", markerMinChar: '౦', markerMaxChar: '౯');
            yield return new OrderedListItemParameters(listStyle: "tibetan", markerMinChar: '༠', markerMaxChar: '༩');
            yield return new OrderedListItemParameters(listStyle: "mongolian", markerMinChar: '᠐', markerMaxChar: '᠙');
            yield return new OrderedListItemParameters(listStyle: "cjk-decimal", markerMinChar: '〇', markerMaxChar: '九');
            yield return new OrderedListItemParameters(listStyle: "fullwidth-decimal", markerMinChar: '０', markerMaxChar: '９');
        }

        private static IBlockDelimiterHandler CreateNumericHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            return new NumericListItemHandler(settings, parameters, delimiter);
        }

        private void AddAlphaHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings)
        {
            AddHandlers(handlers, settings, GetAlphaParameters, CreateAlphaHandler, (int)fancyListsSettings.AlphaListStyles, (int)AlphaListStyles.All);
        }

        private static IEnumerable<OrderedListItemParameters> GetAlphaParameters()
        {
            yield return GetLowerGreekParameters();
            yield return GetLowerRussianParameters();
            yield return GetUpperRussianParameters();
            yield return GetHiraganaParameters();
            yield return GetKatakanaParameters();
        }

        private static OrderedListItemParameters GetLowerGreekParameters()
        {
            return new OrderedListItemParameters(listStyle: "lower-greek", markers: new[]
            {
                new OrderedListMarkerRangeParameters('α', 'ρ'),
                new OrderedListMarkerRangeParameters('σ', 'ω'),
            },
            delimiters: ListItemDelimiterParameters.DefaultLower);
        }

        private static OrderedListItemParameters GetLowerRussianParameters()
        {
            return new OrderedListItemParameters(listStyle: "lower-russian", markers: new[]
            {
                new OrderedListMarkerRangeParameters('а', 'е'),
                new OrderedListMarkerRangeParameters('ж', 'и'),
                new OrderedListMarkerRangeParameters('к', 'щ'),
                new OrderedListMarkerRangeParameters('э', 'я'),
            },
            delimiters: ListItemDelimiterParameters.DefaultLower);
        }

        private static OrderedListItemParameters GetUpperRussianParameters()
        {
            return new OrderedListItemParameters(listStyle: "upper-russian", markers: new[]
            {
                new OrderedListMarkerRangeParameters('А', 'Е'),
                new OrderedListMarkerRangeParameters('Ж', 'И'),
                new OrderedListMarkerRangeParameters('К', 'Щ'),
                new OrderedListMarkerRangeParameters('Э', 'Я'),
            },
            delimiters: ListItemDelimiterParameters.DefaultUpper);
        }

        private static OrderedListItemParameters GetHiraganaParameters()
        {
            return new OrderedListItemParameters(
                listStyle: "hiragana",
                markerChars: new[] { 'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', 'た', 'ち', 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ひ', 'ふ', 'へ', 'ほ', 'ま', 'み', 'む', 'め', 'も', 'や', 'ゆ', 'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'わ', 'を', 'ん' },
                delimiterChars: '、');
        }

        private static OrderedListItemParameters GetKatakanaParameters()
        {
            return new OrderedListItemParameters(
                listStyle: "katakana",
                markerChars: new[] { 'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', 'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', 'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン' },
                delimiterChars: '、');
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
                new OrderedListMarkerRangeParameters('א', 'י'),
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
            valueBase: 1,
            delimiters: ListItemDelimiterParameters.DefaultUpper);
        }

        private static IBlockDelimiterHandler CreateAlphaHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            return new AlphaListItemHandler(settings, parameters, delimiter);
        }

        private static List<IBlockDelimiterHandler> AddHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings, Func<IEnumerable<UnorderedListItemParameters>> paramsFactory,
            Func<CommonMarkSettings, UnorderedListItemParameters, UnorderedListItemDelimiterParameters, IBlockDelimiterHandler> handlerFactory, int flags, int flagsMask)
        {
            Func<UnorderedListItemParameters, UnorderedListItemDelimiterParameters, IBlockDelimiterHandler> itemFactory = (p, d) => handlerFactory(settings, p, d);
            handlers.AddRange(GetItems(paramsFactory, itemFactory, flags, flagsMask));
            return handlers;
        }

        private static List<IBlockDelimiterHandler> AddHandlers(List<IBlockDelimiterHandler> handlers, CommonMarkSettings settings, Func<IEnumerable<OrderedListItemParameters>> paramsFactory,
            Func<CommonMarkSettings, OrderedListItemParameters, ListItemDelimiterParameters, IBlockDelimiterHandler> handlerFactory, int flags, int flagsMask)
        {
            Func<OrderedListItemParameters, ListItemDelimiterParameters, IBlockDelimiterHandler> itemFactory = (p, d) => handlerFactory(settings, p, d);
            handlers.AddRange(GetItems(paramsFactory, itemFactory, flags, flagsMask));
            return handlers;
        }

        private static IEnumerable<char> GetCharacters(Func<IEnumerable<UnorderedListItemParameters>> paramsFactory, int flags, int flagsMask)
        {
            Func<UnorderedListItemParameters, UnorderedListItemDelimiterParameters, char> itemFactory = (p, d) => d.Character;
            return GetItems(paramsFactory, itemFactory, flags, flagsMask);
        }

        private static IEnumerable<T> GetItems<TParameters, TDelimiter, T>(Func<IEnumerable<TParameters>> paramsFactory, Func<TParameters, TDelimiter, T> itemFactory, int flags, int flagsMask)
            where TParameters : ListItemParameters<TDelimiter>
            where TDelimiter : ListItemDelimiterParameters
        {
            if (0 != (flags & flagsMask))
            {
                var mask = flagsMask & ~(flagsMask << 1);
                var source = paramsFactory();
                foreach (var parameters in source)
                {
                    if (0 != (flags & mask))
                    {
                        foreach (var delimiter in parameters.Delimiters)
                            yield return itemFactory(parameters, delimiter);
                    }
                    mask <<= 1;
                }
            }
        }
    }
}
