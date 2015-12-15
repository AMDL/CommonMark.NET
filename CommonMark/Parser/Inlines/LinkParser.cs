using System;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// <see cref="Syntax.InlineTag.Link"/> element parser.
    /// </summary>
    public class LinkParser : InlineParser
    {
        /// <summary>
        /// List of valid schemes of an URL. The array must be sorted.
        /// </summary>
        private static readonly string[] schemeArray = new[] { "AAA", "AAAS", "ABOUT", "ACAP", "ADIUMXTRA", "AFP", "AFS", "AIM", "APT", "ATTACHMENT", "AW", "BESHARE", "BITCOIN", "BOLO", "CALLTO", "CAP", "CHROME", "CHROME-EXTENSION", "CID", "COAP", "COM-EVENTBRITE-ATTENDEE", "CONTENT", "CRID", "CVS", "DATA", "DAV", "DICT", "DLNA-PLAYCONTAINER", "DLNA-PLAYSINGLE", "DNS", "DOI", "DTN", "DVB", "ED2K", "FACETIME", "FEED", "FILE", "FINGER", "FISH", "FTP", "GEO", "GG", "GIT", "GIZMOPROJECT", "GO", "GOPHER", "GTALK", "H323", "HCP", "HTTP", "HTTPS", "IAX", "ICAP", "ICON", "IM", "IMAP", "INFO", "IPN", "IPP", "IRC", "IRC6", "IRCS", "IRIS", "IRIS.BEEP", "IRIS.LWZ", "IRIS.XPC", "IRIS.XPCS", "ITMS", "JAR", "JAVASCRIPT", "JMS", "KEYPARC", "LASTFM", "LDAP", "LDAPS", "MAGNET", "MAILTO", "MAPS", "MARKET", "MESSAGE", "MID", "MMS", "MS-HELP", "MSNIM", "MSRP", "MSRPS", "MTQP", "MUMBLE", "MUPDATE", "MVN", "NEWS", "NFS", "NI", "NIH", "NNTP", "NOTES", "OID", "OPAQUELOCKTOKEN", "PALM", "PAPARAZZI", "PLATFORM", "POP", "PRES", "PROXY", "PSYC", "QUERY", "RES", "RESOURCE", "RMI", "RSYNC", "RTMP", "RTSP", "SECONDLIFE", "SERVICE", "SESSION", "SFTP", "SGN", "SHTTP", "SIEVE", "SIP", "SIPS", "SKYPE", "SMB", "SMS", "SNMP", "SOAP.BEEP", "SOAP.BEEPS", "SOLDAT", "SPOTIFY", "SSH", "STEAM", "SVN", "TAG", "TEAMSPEAK", "TEL", "TELNET", "TFTP", "THINGS", "THISMESSAGE", "TIP", "TN3270", "TV", "UDP", "UNREAL", "URN", "UT2004", "VEMMI", "VENTRILO", "VIEW-SOURCE", "WEBCAL", "WS", "WSS", "WTAI", "WYCIWYG", "XCON", "XCON-USERID", "XFIRE", "XMLRPC.BEEP", "XMLRPC.BEEPS", "XMPP", "XRI", "YMSGR", "Z39.50R", "Z39.50S" };

        /// <summary>
        /// Gets the opening characters that are handled by this parser.
        /// </summary>
        /// <value>Array containing the characters that can open a handled element.</value>
        public override char[] Characters
        {
            get { return new[] { '<' }; }
        }

        /// <summary>
        /// Try to match URI autolink after first &lt;, returning number of chars matched.
        /// </summary>
        /// <remarks>Original: int scan_autolink_uri(string s, int pos, int sourceLength)</remarks>
        internal static int ScanAutolinkUri(string s, int pos, int sourceLength)
        {
            /*!re2c
              scheme [:]([^\x00-\x20<>\\]|escaped_char)*[>]  { return (p - start); }
              .? { return 0; }
            */
            // for now the tests do not include anything that would require the use of `escaped_char` part so it is ignored.

            // 24 is the maximum length of a valid scheme
            var checkLen = sourceLength - pos;
            if (checkLen > 24)
                checkLen = 24;

            // PERF: potential small improvement - instead of using IndexOf, check char-by-char and return as soon as an invalid character is found ([^a-z0-9\.])
            // alternative approach (if we want to go crazy about performance - store the valid schemes as a prefix tree and lookup the valid scheme char by char and
            // return as soon as the part does not match any prefix.
            var colonpos = s.IndexOf(':', pos, checkLen);
            if (colonpos == -1)
                return 0;

            var potentialScheme = s.Substring(pos, colonpos - pos).ToUpperInvariant();
            if (Array.BinarySearch(schemeArray, potentialScheme, StringComparer.Ordinal) < 0)
                return 0;

            for (var i = colonpos + 1; i < sourceLength; i++)
            {
                var c = s[i];
                if (c == '>')
                    return i - pos + 1;

                if (c == '<' || c <= 0x20)
                    return 0;
            }

            return 0;
        }

        /// <summary>
        /// Try to match email autolink after first &lt;, returning num of chars matched.
        /// </summary>
        /// <remarks>Original: int scan_autolink_email(string s, int pos, int sourceLength)</remarks>
        internal static int ScanAutolinkEmail(string s, int pos, int sourceLength)
        {
            /*!re2c
              [a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+
                [@]
                [a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
                ([.][a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*
                [>] { return (p - start); }
              .? { return 0; }
            */

            if (pos + 6 >= sourceLength)
                return 0;

            char c = s[pos];
            if (c == '@')
                return 0;

            int i = pos;
            int ln = sourceLength - 1;
            while (i <= ln)
            {
                if (c == '@')
                    break;

                if ((c < 'a' || c > 'z')
                    && (c < 'A' || c > 'Z')
                    && (c < '0' || c > '9')
                    && ".!#$%&'*+/=?^_`{|}~-".IndexOf(c) == -1)
                    return 0;
                if (i == ln) return 0;
                c = s[++i];
            }

            // move past '@'
            if (i == ln) return 0;
            c = s[++i];
            bool hadDot = false;

            while (true)
            {
                var domainStart = i;
                if (!ScannerCharacterMatcher.MatchAsciiLetterOrDigit(s, ref c, ref i, ln, '-'))
                    return 0;

                if (s[i - 1] == '-' || i - domainStart > 63)
                    return 0;

                if (c == '>')
                    return hadDot ? i - pos + 1 : 0;

                if (c != '.' || i == ln)
                    return 0;

                hadDot = true;
                c = s[++i];
            }
        }
    }
}
