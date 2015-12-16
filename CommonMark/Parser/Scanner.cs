using System;

namespace CommonMark.Parser
{
    /// <summary>
    /// Contains the regular expressions that are used in the parsers.
    /// </summary>
    internal static class Scanner
    {

        /// <summary>
        /// Try to match a URL in a link or reference, return number of chars matched.
        /// This may optionally be contained in &lt;..&gt;; otherwise
        /// whitespace and unbalanced right parentheses aren't allowed.
        /// Newlines aren't ever allowed.
        /// </summary>
        public static int scan_link_url(string s, int pos, int sourceLength)
        {
            /*!re2c
              [ \n]* [<] ([^<>\n\\\x00] | escaped_char | [\\])* [>] { return (p - start); }
              [ \n]* (reg_char+ | escaped_char | in_parens_nosp)* { return (p - start); }
              .? { return 0; }
            */

            if (pos + 1 >= sourceLength)
                return 0;

            var i = pos;
            var c = s[i];
            var nextEscaped = false;
            var lastPos = sourceLength - 1;
            // move past any whitespaces
            ScannerCharacterMatcher.MatchWhitespaces(s, ref c, ref i, lastPos);

            if (c == '<')
            {
                if (i == lastPos) return 0;
                c = s[++i];
                while (i <= lastPos)
                {
                    if (c == '\n') return 0;
                    if (c == '<' && !nextEscaped) return 0;
                    if (c == '>' && !nextEscaped) return i - pos + 1;
                    if (i == lastPos) return 0;
                    nextEscaped = !nextEscaped && c == '\\';
                    c = s[++i];
                }
                return 0;
            }

            bool openParens = false;
            while (i <= lastPos)
            {
                if (c == '(' && !nextEscaped)
                {
                    if (openParens)
                        return 0;
                    openParens = true;
                }
                if (c == ')' && !nextEscaped)
                {
                    if (!openParens)
                        return i - pos;
                    openParens = false;
                }
                if (c <= 0x20)
                    return openParens ? 0 : i - pos;

                if (i == lastPos)
                    return openParens ? 0 : i - pos + 1;

                nextEscaped = !nextEscaped && c == '\\';
                c = s[++i];
            }

            return 0;
        }

        /// <summary>
        /// Try to match a link title (in single quotes, in double quotes, or
        /// in parentheses), returning number of chars matched.  Allow one
        /// level of internal nesting (quotes within quotes).
        /// </summary>
        public static int scan_link_title(string s, int pos, int sourceLength)
        {
            /*!re2c
              ["] (escaped_char|[^"\x00])* ["]   { return (p - start); }
              ['] (escaped_char|[^'\x00])* ['] { return (p - start); }
              [(] (escaped_char|[^)\x00])* [)]  { return (p - start); }
              .? { return 0; }
            */

            if (pos + 2 >= sourceLength)
                return 0;

            var c1 = s[pos];
            if (c1 != '"' && c1 != '\'' && c1 != '(')
                return 0;

            if (c1 == '(') c1 = ')';

            var nextEscaped = false;
            for (var i = pos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == c1 && !nextEscaped)
                    return i - pos + 1;

                nextEscaped = !nextEscaped && c == '\\';
            }

            return 0;
        }

        /// <summary>
        /// Scans an entity.
        /// Returns number of chars matched.
        /// </summary>
        public static int scan_entity(string s, int pos, int length, out string namedEntity, out int numericEntity)
        {
            /*!re2c
              [&] ([#] ([Xx][A-Fa-f0-9]{1,8}|[0-9]{1,8}) |[A-Za-z][A-Za-z0-9]{1,31} ) [;]
                 { return (p - start); }
              .? { return 0; }
            */

            var lastPos = pos + length;

            namedEntity = null;
            numericEntity = 0;

            if (pos + 3 >= lastPos)
                return 0;

            if (s[pos] != '&')
                return 0;

            char c;
            int i;
            int counter = 0;
            if (s[pos + 1] == '#')
            {
                c = s[pos + 2];
                if (c == 'x' || c == 'X')
                {
                    // expect 1-8 hex digits starting from pos+3
                    for (i = pos + 3; i < lastPos; i++)
                    {
                        c = s[i];
                        if (c >= '0' && c <= '9')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity * 16 + (c - '0');
                            continue;
                        }
                        else if (c >= 'A' && c <= 'F')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity * 16 + (c - 'A' + 10);
                            continue;
                        }
                        else if (c >= 'a' && c <= 'f')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity * 16 + (c - 'a' + 10);
                            continue;
                        }

                        if (c == ';')
                            return counter == 0 ? 0 : i - pos + 1;

                        return 0;
                    }
                }
                else
                {
                    // expect 1-8 digits starting from pos+2
                    for (i = pos + 2; i < lastPos; i++)
                    {
                        c = s[i];
                        if (c >= '0' && c <= '9')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity * 10 + (c - '0');
                            continue;
                        }

                        if (c == ';')
                            return counter == 0 ? 0 : i - pos + 1;

                        return 0;
                    }
                }
            }
            else
            {
                // expect a letter and 1-31 letters or digits
                c = s[pos + 1];
                if ((c < 'A' || c > 'Z') && (c < 'a' && c > 'z'))
                    return 0;

                for (i = pos + 2; i < lastPos; i++)
                {
                    c = s[i];
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        if (++counter == 32)
                            return 0;

                        continue;
                    }

                    if (c == ';')
                    {
                        namedEntity = s.Substring(pos + 1, counter + 1);
                        return counter == 0 ? 0 : i - pos + 1;
                    }

                    return 0;
                }
            }

            return 0;
        }
    }
}
