using System.Collections.Generic;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Numeric ordered list item delimiter handler.
    /// </summary>
    public sealed class NumericListItemHandler : OrderedListItemHandler
    {
        /// <summary>
        /// Creates numeric list item delimiter handlers using the specified parameters.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">List item parameters.</param>
        /// <returns>A collection of numeric list item delimiter handlers.</returns>
        public static IEnumerable<IBlockDelimiterHandler> Create(CommonMarkSettings settings, OrderedListItemParameters parameters)
        {
            if (parameters != null && parameters.Markers.Length == 1)
            {
                var range = parameters.Markers[0] as OrderedListMarkerRangeParameters;
                for (var i = 0; i <= range.MaxCharacter - range.MinCharacter; i++)
                {
                    yield return new NumericListItemHandler(settings, (char)(i + range.MinCharacter), range, parameters);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="character">Handled character.</param>
        /// <param name="range">Marker range parameters.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        public NumericListItemHandler(CommonMarkSettings settings, char character, OrderedListMarkerRangeParameters range, OrderedListItemParameters parameters)
            : base(settings, character, range.MinCharacter, range.MaxCharacter, false, parameters)
        {
            StartValue = range.StartValue;
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

        /// <summary>
        /// Adjust the start value.
        /// </summary>
        /// <param name="start">Current start value.</param>
        /// <param name="value">Current character value.</param>
        /// <param name="curChar">Current character.</param>
        /// <returns><c>true</c> if successful.</returns>
        protected override bool AdjustStart(ref int start, ref int value, char curChar)
        {
            value = curChar - MarkerMinCharacter + StartValue;
            start = start * ValueBase + value;
            return true;
        }

        private int StartValue
        {
            get;
        }
    }
}
