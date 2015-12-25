namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Base single-range ordered list item delimiter handler class.
    /// </summary>
    public abstract class SingleRangeListItemHandler : OrderedListItemHandler<SingleRangeListItemHandler.Parameters>
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
            /// <param name="isRequireContent"><c>true</c> if items on this list require content.</param>
            public Parameters(OrderedListItemParameters parameters, char[] characters, bool isRequireContent)
                : base(parameters, characters, isRequireContent)
            {
            }

            /// <summary>
            /// Gets or sets the start value.
            /// </summary>
            public int StartValue { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleRangeListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        /// <param name="isRequireContent"><c>true</c> if items on this list require content.</param>
        protected SingleRangeListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters, bool isRequireContent)
            : base(settings, GetHandlerParameters(parameters, isRequireContent))
        {
            StartValue = HandlerParameters.StartValue;
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

        private static Parameters GetHandlerParameters(OrderedListItemParameters parameters, bool isRequireContent)
        {
            var range = parameters.Markers[0] as OrderedListMarkerRangeParameters;
            var length = range.MaxCharacter - range.MinCharacter + 1;
            var characters = new char[length];
            for (var i = 0; i < length; i++)
            {
                characters[i] = (char)(i + range.MinCharacter);
            }

            return new Parameters(parameters, characters, isRequireContent)
            {
                MarkerMinChar = range.MinCharacter,
                MarkerMaxChar = range.MaxCharacter,
                StartValue = range.StartValue,
                ValueBase = range.MaxCharacter - range.MinCharacter + 1,
            };
        }
    }
}
