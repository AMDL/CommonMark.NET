using System.Collections.Generic;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Base char-to-value mapping ordered list item delimiter handler class.
    /// </summary>
    public abstract class MappingListItemHandler : OrderedListItemHandler
    {
        /// <summary>
        /// Creates a mapping from character to value.
        /// </summary>
        /// <param name="markers">Marker parameters.</param>
        /// <param name="valueMapDict">Value map dictionary.</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <returns></returns>
        protected static int[] CreateValueMap(OrderedListMarkerParameters[] markers, Dictionary<char, int> valueMapDict, out char markerMinChar, out char markerMaxChar)
        {
            markerMinChar = char.MaxValue;
            markerMaxChar = char.MinValue;
            foreach (var marker in markers)
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

            return valueMap;
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
                valueMap.Add((char)(i + rangeMin), range.StartValue + i);
            }
            if (rangeMin < min)
                min = rangeMin;
            if (rangeMax > max)
                max = rangeMax;

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="valueMap">Character to value mapping (<paramref name="markerMinChar"/>-based).</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        protected MappingListItemHandler(CommonMarkSettings settings, char character, int[] valueMap, char markerMinChar, char markerMaxChar, OrderedListItemParameters parameters)
            : base(settings, character, markerMinChar, markerMaxChar, true, parameters)
        {
            this.ValueMap = valueMap;
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
    }
}
