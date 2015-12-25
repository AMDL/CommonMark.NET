namespace CommonMark
{
    /// <summary>
    /// Character type.
    /// </summary>
    public struct CharacterType
    {
        /// <summary>
        /// Gets or sets the value indicating whether the character is a space character.
        /// </summary>
        public bool IsSpace;

        /// <summary>
        /// Gets or sets the value indicating whether the character is a punctuation character.
        /// </summary>
        public bool IsPunctuation;

        /// <summary>
        /// Gets or sets the value indicating whether the character is a decimal digit.
        /// </summary>
        public bool IsDigit;
    }

    /// <summary>
    /// Reusable utility functions, not directly related to parsing or formatting data.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Writes a warning to the Debug window.
        /// </summary>
        /// <param name="message">The message with optional formatting placeholders.</param>
        /// <param name="args">The arguments for the formatting placeholders.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Warning(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = string.Format(System.Globalization.CultureInfo.InvariantCulture, message, args);

            System.Diagnostics.Debug.WriteLine(message, "Warning");
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsEscapableSymbol(char c)
        {
            // char.IsSymbol also works with Unicode symbols that cannot be escaped based on the specification.
            return (c > ' ' && c < '0') || (c > '9' && c < 'A') || (c > 'Z' && c < 'a') || (c > 'z' && c < 127)
                || c == '•' || c == 'o' || c == '';
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsAsciiLetterOrDigit(char c)
        {
            // char.IsSymbol also works with Unicode symbols that cannot be escaped based on the specification.
            return (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z');
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\f';
        }

        /// <summary>
        /// Checks if the given character is a decimal digit, an Unicode space, or a punctuation character.
        /// </summary>
        /// <remarks>Original: void CheckUnicodeCategory(char c, out bool space, out bool punctuation)</remarks>
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static CharacterType GetCharacterType(char c)
        {
            // This method does the same as would calling the three built-in methods:
            // // digit = char.IsDigit(c);
            // // space = char.IsWhiteSpace(c);
            // // punctuation = char.IsPunctuation(c);
            //
            // The performance benefit for using this method was ~50% when calling only on ASCII characters
            // and ~12% when calling only on Unicode characters (measured before the digit addition).

            bool digit, space, punctuation;
            if (c <= 'ÿ')
            {
                digit = c >= '0' && c <= '9';
                space = c == ' ' || (c >= '\t' && c <= '\r') || c == '\u00a0' || c == '\u0085';
                punctuation = (c >= 33 && c <= 47) || (c >= 58 && c <= 64) || (c >= 91 && c <= 96) || (c >= 123 && c <= 126);
            }
            else
            {
                var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                digit = category == System.Globalization.UnicodeCategory.DecimalDigitNumber;
                space = !digit &&
                    (category == System.Globalization.UnicodeCategory.SpaceSeparator
                    || category == System.Globalization.UnicodeCategory.LineSeparator
                    || category == System.Globalization.UnicodeCategory.ParagraphSeparator);
                punctuation = !space &&
                    (category == System.Globalization.UnicodeCategory.ConnectorPunctuation
                    || category == System.Globalization.UnicodeCategory.DashPunctuation
                    || category == System.Globalization.UnicodeCategory.OpenPunctuation
                    || category == System.Globalization.UnicodeCategory.ClosePunctuation
                    || category == System.Globalization.UnicodeCategory.InitialQuotePunctuation
                    || category == System.Globalization.UnicodeCategory.FinalQuotePunctuation
                    || category == System.Globalization.UnicodeCategory.OtherPunctuation);
            }

            return new CharacterType
            {
                IsDigit = digit,
                IsSpace = space,
                IsPunctuation = punctuation,
            };
        }

        /// <summary>
        /// Determines if the first line (ignoring the first <paramref name="startIndex"/>) of a string contains only spaces.
        /// </summary>
        public static bool IsFirstLineBlank(string source, int startIndex)
        {
            char c;
            var lastIndex = source.Length;
            
            while (startIndex < lastIndex)
            {
                c = source[startIndex];
                if (c == '\n')
                    return true;

                if (c != ' ')
                    return false;

                startIndex++;
            }

            return true;
        }
    }
}
