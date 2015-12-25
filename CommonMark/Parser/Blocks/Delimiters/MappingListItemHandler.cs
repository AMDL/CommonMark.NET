using System.Collections.Generic;

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
            public Parameters(OrderedListItemParameters parameters, char[] characters)
                : base(parameters, characters, true)
            {
            }

            /// <summary>
            /// Gets or sets the value map.
            /// </summary>
            public int[] ValueMap { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        protected MappingListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
            : base(settings, GetHandlerParameters(parameters))
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
        protected override bool AdjustStart(ref int start, ref int value, char curChar)
        {
            if ((value = ValueMap[curChar - MarkerMinCharacter]) == 0)
                return false;
            start = start * ValueBase + value;
            return true;
        }

        /// <summary>
        /// Gets the value map.
        /// </summary>
        protected int[] ValueMap
        {
            get;
        }

        private static Parameters GetHandlerParameters(OrderedListItemParameters parameters)
        {
            var valueMapDict = new Dictionary<char, int>();
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

            var valueMap = new int[markerMaxChar - markerMinChar + 1];
            foreach (var kvp in valueMapDict)
            {
                valueMap[kvp.Key - markerMinChar] = kvp.Value;
            }

            var characters = new char[valueMapDict.Count];
            valueMapDict.Keys.CopyTo(characters, 0);

            return new Parameters(parameters, characters)
            {
                MarkerMinChar = markerMinChar,
                MarkerMaxChar = markerMaxChar,
                ValueMap = valueMap,
                ValueBase = parameters.ValueBase != 0 ? parameters.ValueBase : valueMapDict.Count,
            };
        }

        private static bool AddSingle(OrderedListSingleMarkerParameters single, Dictionary<char, int> valueMap, ref char min, ref char max)
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

        private static bool AddRange(OrderedListMarkerRangeParameters range, Dictionary<char, int> valueMap, ref char min, ref char max)
        {
            if (range == null)
                return false;

            var rangeMin = range.MinCharacter;
            var rangeMax = range.MaxCharacter;
            for (int i = 0; i <= rangeMax - rangeMin; i++)
            {
                valueMap.Add((char)(i + rangeMin), valueMap.Count + 1);
            }
            if (rangeMin < min)
                min = rangeMin;
            if (rangeMax > max)
                max = rangeMax;

            return true;
        }
    }
}
