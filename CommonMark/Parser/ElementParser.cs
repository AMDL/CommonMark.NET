namespace CommonMark.Parser
{
    /// <summary>
    /// Base element parser class.
    /// </summary>
    public abstract class ElementParser
    {
        /// <summary>
        /// Attempts to match an HTML closing tag.
        /// </summary>
        /// <returns>Number of chars matched, or 0 for no match.</returns>
        /// <remarks>Original: int _scanHtmlTagCloseTag(string s, int pos, int sourceLength)</remarks>
        internal static int ScanHtmlCloseTag(string s, int pos, int sourceLength)
        {
            // Original regexp: "[/]" + tagname + @"\s*[>]"

            if (pos + 2 >= sourceLength)
                return 0;

            var nextChar = s[pos + 1];
            if ((nextChar < 'A' || nextChar > 'Z') && (nextChar < 'a' || nextChar > 'z'))
                return 0;

            var tagNameEnded = false;
            for (var i = pos + 2; i < sourceLength; i++)
            {
                nextChar = s[i];
                if (nextChar == '>')
                    return i - pos + 1;

                if (nextChar == ' ' || nextChar == '\n')
                {
                    tagNameEnded = true;
                    continue;
                }

                if (tagNameEnded || ((nextChar < 'A' || nextChar > 'Z')
                                  && (nextChar < 'a' || nextChar > 'z')
                                  && (nextChar < '0' || nextChar > '9')))
                    return 0;
            }

            return 0;
        }

        /// <summary>
        /// Attempts to match an HTML opening tag.
        /// </summary>
        /// <returns>Number of chars matched, or 0 for no match.</returns>
        /// <remarks>Original: int _scanHtmlTagOpenTag(string s, int pos, int sourceLength)</remarks>
        internal static int ScanHtmlOpenTag(string s, int pos, int sourceLength)
        {
            var lastPosition = sourceLength - 1;

            // the minimum length valid tag is "a>"
            if (lastPosition < pos + 1)
                return 0;

            // currentPosition - positioned after the last character matched by that any particular part
            var currentPosition = pos;

            // stores the character at the current position
            char currentChar = s[currentPosition];

            // stores if the previous character was a whitespace
            bool hadWhitespace = false;

            // stores if an attribute name has been parsed
            bool hadAttribute = false;

            // some additional variables used in the process
            char c1;

            // The tag name must start with an ASCII letter
            if (!ScannerCharacterMatcher.MatchAsciiLetter(s, ref currentChar, ref currentPosition, lastPosition))
                return 0;

            // Move past any other characters that make up the tag name
            ScannerCharacterMatcher.MatchHtmlTagNameCharacter(s, ref currentChar, ref currentPosition, lastPosition);

            // loop while the end of string is reached or the tag is closed
            while (currentPosition <= lastPosition)
            {
                // Move past any whitespaces
                hadWhitespace = ScannerCharacterMatcher.MatchWhitespaces(s, ref currentChar, ref currentPosition, lastPosition);

                // check if the end of the tag has been reached
                if (currentChar == '>')
                    return currentPosition - pos + 1;

                if (currentChar == '/')
                {
                    if (currentPosition == lastPosition) return 0;
                    currentChar = s[++currentPosition];
                    return (currentChar == '>') ? currentPosition - pos + 1 : 0;
                }

                // check if arrived at the attribute value
                if (currentChar == '=')
                {
                    if (!hadAttribute || currentPosition == lastPosition)
                        return 0;

                    // move past the '=' symbol and any whitespaces
                    currentChar = s[++currentPosition];
                    ScannerCharacterMatcher.MatchWhitespaces(s, ref currentChar, ref currentPosition, lastPosition);

                    if (currentChar == '\'' || currentChar == '\"')
                    {
                        c1 = currentChar;

                        currentChar = s[++currentPosition];
                        ScannerCharacterMatcher.MatchAnythingExcept(s, ref currentChar, ref currentPosition, lastPosition, c1);

                        if (currentChar != c1 || currentPosition == lastPosition)
                            return 0;

                        currentChar = s[++currentPosition];
                    }
                    else
                    {
                        // an unquoted value must have at least one character
                        if (!ScannerCharacterMatcher.MatchAnythingExceptWhitespaces(s, ref currentChar, ref currentPosition, lastPosition, '\"', '\'', '=', '<', '>', '`'))
                            return 0;
                    }

                    hadAttribute = false;
                    continue;
                }

                // the attribute must be preceded by a whitespace
                if (!hadWhitespace)
                    return 0;

                // if the end has not been found then there is just one possible alternative - an attribute
                // validate that the attribute name starts with a correct character
                if (!ScannerCharacterMatcher.MatchAsciiLetter(s, ref currentChar, ref currentPosition, lastPosition, '_', ':'))
                    return 0;

                // match any remaining characters in the attribute name
                ScannerCharacterMatcher.MatchAsciiLetterOrDigit(s, ref currentChar, ref currentPosition, lastPosition, '_', ':', '.', '-');

                hadAttribute = true;
            }

            return 0;
        }

        /// <summary>
        /// Determines whether the specified object has the same type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if the object is an instance of the same type.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && this.GetType().Equals(obj.GetType());
        }

        /// <summary>
        /// Returns the hash code of the type object.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
