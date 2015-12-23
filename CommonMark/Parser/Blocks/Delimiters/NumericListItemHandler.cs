using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser.Blocks.Delimiters
{
    /// <summary>
    /// Numeric ordered list item delimiter handler.
    /// </summary>
    public sealed class NumericListItemHandler : OrderedListItemHandler<NumericListItemHandler.Parameters>
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
                : base(parameters, characters, false)
            {
            }

            /// <summary>
            /// Gets or sets the start value.
            /// </summary>
            public int StartValue { get; set; }
        }

        /// <summary>
        /// The default parameters instance.
        /// </summary>
        public static readonly OrderedListItemParameters DefaultParameters = new OrderedListItemParameters(
            '0', '9', 0, 10, 9,
            BlockTag.ListItem,
            BlockTag.OrderedList,
#pragma warning disable 0618
            ListType.Ordered,
#pragma warning restore 0618
            OrderedListMarkerType.None,
            null,
            new ListItemDelimiterParameters('.'),
            new ListItemDelimiterParameters(')'));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericListItemHandler"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        /// <param name="parameters">Ordered list parameters.</param>
        public NumericListItemHandler(CommonMarkSettings settings, OrderedListItemParameters parameters)
            : base(settings, GetHandlerParameters(parameters))
        {
            StartValue = HandlerParameters.StartValue;
        }

        /// <summary>
        /// Handles a list item opener.
        /// </summary>
        /// <param name="info">Parser state.</param>
        /// <returns><c>true</c> if successful.</returns>
        public override bool Handle(ref BlockParserInfo info)
        {
            return DoHandle(info, CanOpen, ParseMarker, AdjustStart, MatchList, SetListData);
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

        private static Parameters GetHandlerParameters(OrderedListItemParameters parameters)
        {
            var range = parameters.Markers[0] as OrderedListMarkerRangeParameters;
            var length = range.MaxCharacter - range.MinCharacter + 1;
            var characters = new char[length];
            for (var i = 0; i < length; i++)
            {
                characters[i] = (char)(i + range.MinCharacter);
            }

            return new Parameters(parameters, characters)
            {
                MarkerMinChar = range.MinCharacter,
                MarkerMaxChar = range.MaxCharacter,
                StartValue = range.StartValue,
            };
        }
    }
}
