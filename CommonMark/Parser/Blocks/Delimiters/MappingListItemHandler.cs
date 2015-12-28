﻿using System.Collections.Generic;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Base char-to-value mapping ordered list item delimiter handler class.
    /// </summary>
    public abstract class MappingListItemHandler : OrderedListItemHandler<MappingListItemHandler.Parameters>
    {
        /// <summary>
        /// Handler parameters.
        /// </summary>
        public new sealed class Parameters : OrderedListItemHandler<Parameters>.Parameters
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Parameters"/> class.
            /// </summary>
            /// <param name="parameters">List item parameters.</param>
            /// <param name="characters">Handled characters.</param>
            /// <param name="delimiter">Delimiter parameters.</param>
            public Parameters(OrderedListItemParameters parameters, char[] characters, ListItemDelimiterParameters delimiter)
                : base(parameters, characters, delimiter, true)
            {
            }

            /// <summary>
            /// Gets or sets the value map.
            /// </summary>
            public short[] ValueMap { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        /// <param name="delimiter">Delimiter parameters.</param>
        protected MappingListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
            : base(settings, GetHandlerParameters(parameters, delimiter))
        {
            this.ValueMap = HandlerParameters.ValueMap;
        }

        /// <summary>
        /// Adjust the start value.
        /// </summary>
        /// <param name="start">Current start value.</param>
        /// <param name="value">Current character value.</param>
        /// <param name="curChar">Current character.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected override bool AdjustStart(ref int start, ref short value, char curChar)
        {
            if ((value = ValueMap[curChar - MarkerMinCharacter]) == 0)
                return false;
            start = start * ValueBase + value;
            return true;
        }

        /// <summary>
        /// Gets the value map.
        /// </summary>
        protected short[] ValueMap
        {
            get;
        }

        private static Parameters GetHandlerParameters(OrderedListItemParameters parameters, ListItemDelimiterParameters delimiter)
        {
            var valueMapDict = new Dictionary<char, short>();
            var markerMinChar = char.MaxValue;
            var markerMaxChar = char.MinValue;
            foreach (var marker in parameters.Markers)
            {
                if (!AddSingle(marker as OrderedListSingleMarkerParameters, valueMapDict, ref markerMinChar, ref markerMaxChar)
                    && !AddRange(marker as OrderedListMarkerRangeParameters, valueMapDict, ref markerMinChar, ref markerMaxChar))
                {
                    throw new System.InvalidOperationException();
                }
            }

            var valueMap = new short[markerMaxChar - markerMinChar + 1];
            short count = 0;
            foreach (var kvp in valueMapDict)
            {
                ++count;
                valueMap[kvp.Key - markerMinChar] = kvp.Value != 0 ? kvp.Value : count;
            }

            var characters = new char[count];
            valueMapDict.Keys.CopyTo(characters, 0);

            return new Parameters(parameters, characters, delimiter)
            {
                MarkerMinChar = markerMinChar,
                MarkerMaxChar = markerMaxChar,
                ValueMap = valueMap,
                ValueBase = parameters.ValueBase != 0 ? parameters.ValueBase : count,
            };
        }

        private static bool AddSingle(OrderedListSingleMarkerParameters single, Dictionary<char, short> valueMap, ref char min, ref char max)
        {
            if (single == null)
                return false;

            var singleChar = single.Character;
            valueMap.Add(singleChar, single.StartValue);
            if (singleChar < min)
                min = singleChar;
            if (singleChar > max)
                max = singleChar;

            return true;
        }

        private static bool AddRange(OrderedListMarkerRangeParameters range, Dictionary<char, short> valueMap, ref char min, ref char max)
        {
            if (range == null)
                return false;

            var rangeMin = range.MinCharacter;
            var rangeMax = range.MaxCharacter;
            var value = range.StartValue > 0 ? range.StartValue : (short)(valueMap.Count + 1);
            var factor = range.Factor;
            for (int i = 0; i <= rangeMax - rangeMin; i++)
            {
                valueMap.Add((char)(i + rangeMin), value);
                value += factor;
            }
            if (rangeMin < min)
                min = rangeMin;
            if (rangeMax > max)
                max = rangeMax;

            return true;
        }
    }
}
