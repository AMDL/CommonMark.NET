using System.Collections.Generic;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Alphabetical ordered list item delimiter handler.
    /// </summary>
    public sealed class AlphaListItemHandler : MappingListItemHandler
    {
        /// <summary>
        /// Creates alphabetical list item delimiter handlers using the specified parameters.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <returns>A collection of alphabetical list item delimiter handlers.</returns>
        public static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            char min;
            char max;
            var valueMapDict = new Dictionary<char, int>();
            var valueMap = CreateValueMap(parameters.Markers, valueMapDict, out min, out max);

            foreach (var kvp in valueMapDict)
            {
                yield return new AlphaListItemHandler(settings, kvp.Key, valueMap, min, max, parameters);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="valueMap">Character to value mapping (<paramref name="markerMinChar"/>-based).</param>
        /// <param name="markerMinChar">First marker character.</param>
        /// <param name="markerMaxChar">Last marker character.</param>
        /// <param name="parameters">Ordered list item parameters.</param>
        public AlphaListItemHandler(CommonMarkSettings settings, char character, int[] valueMap, char markerMinChar, char markerMaxChar, OrderedListItemParameters parameters)
            : base(settings, character, valueMap, markerMinChar, markerMaxChar, parameters)
        {
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, AdjustStart, IsListsMatch, SetListData);
        }
    }
}
