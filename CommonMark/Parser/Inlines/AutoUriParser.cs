using CommonMark.Syntax;
using System;

namespace CommonMark.Parser.Inlines
{
    /// <summary>
    /// Automatic URI link parser.
    /// </summary>
    public class AutoUriParser : InlineParser
    {
        /// <summary>
        /// List of valid schemes of an URL. The array must be sorted.
        /// </summary>
        private static readonly string[] schemeArray = new[] { "AAA", "AAAS", "ABOUT", "ACAP", "ADIUMXTRA", "AFP", "AFS", "AIM", "APT", "ATTACHMENT", "AW", "BESHARE", "BITCOIN", "BOLO", "CALLTO", "CAP", "CHROME", "CHROME-EXTENSION", "CID", "COAP", "COM-EVENTBRITE-ATTENDEE", "CONTENT", "CRID", "CVS", "DATA", "DAV", "DICT", "DLNA-PLAYCONTAINER", "DLNA-PLAYSINGLE", "DNS", "DOI", "DTN", "DVB", "ED2K", "FACETIME", "FEED", "FILE", "FINGER", "FISH", "FTP", "GEO", "GG", "GIT", "GIZMOPROJECT", "GO", "GOPHER", "GTALK", "H323", "HCP", "HTTP", "HTTPS", "IAX", "ICAP", "ICON", "IM", "IMAP", "INFO", "IPN", "IPP", "IRC", "IRC6", "IRCS", "IRIS", "IRIS.BEEP", "IRIS.LWZ", "IRIS.XPC", "IRIS.XPCS", "ITMS", "JAR", "JAVASCRIPT", "JMS", "KEYPARC", "LASTFM", "LDAP", "LDAPS", "MAGNET", "MAILTO", "MAPS", "MARKET", "MESSAGE", "MID", "MMS", "MS-HELP", "MSNIM", "MSRP", "MSRPS", "MTQP", "MUMBLE", "MUPDATE", "MVN", "NEWS", "NFS", "NI", "NIH", "NNTP", "NOTES", "OID", "OPAQUELOCKTOKEN", "PALM", "PAPARAZZI", "PLATFORM", "POP", "PRES", "PROXY", "PSYC", "QUERY", "RES", "RESOURCE", "RMI", "RSYNC", "RTMP", "RTSP", "SECONDLIFE", "SERVICE", "SESSION", "SFTP", "SGN", "SHTTP", "SIEVE", "SIP", "SIPS", "SKYPE", "SMB", "SMS", "SNMP", "SOAP.BEEP", "SOAP.BEEPS", "SOLDAT", "SPOTIFY", "SSH", "STEAM", "SVN", "TAG", "TEAMSPEAK", "TEL", "TELNET", "TFTP", "THINGS", "THISMESSAGE", "TIP", "TN3270", "TV", "UDP", "UNREAL", "URN", "UT2004", "VEMMI", "VENTRILO", "VIEW-SOURCE", "WEBCAL", "WS", "WSS", "WTAI", "WYCIWYG", "XCON", "XCON-USERID", "XFIRE", "XMLRPC.BEEP", "XMLRPC.BEEPS", "XMPP", "XRI", "YMSGR", "Z39.50R", "Z39.50S" };

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUriParser"/> class.
        /// </summary>
        /// <param name="settings">Common settings.</param>
        public AutoUriParser(CommonMarkSettings settings)
            : base(settings, '<')
        {
        }

        /// <summary>
        /// Attempts to match a URL autolink.
        /// </summary>
        /// <param name="container">Parent container.</param>
        /// <param name="subj">Subject.</param>
        /// <returns>Inline element or <c>null</c>.</returns>
        public override Inline Handle(Block container, Subject subj)
        {
            // advance past first <
            subj.Position++;

            var matchlen = Scan(subj.Buffer, subj.Position, subj.Length);
            if (matchlen == 0)
            {
                subj.Position--;
                return null;
            }

            var contents = subj.Buffer.Substring(subj.Position, matchlen - 1);
            var resultContents = ParseStringEntities(contents);
            return CreateLink(subj, resultContents, contents, string.Empty, matchlen);
        }

        /// <summary>
        /// Try to match URI autolink after first &lt;, returning number of chars matched.
        /// </summary>
        /// <remarks>Original: int scan_autolink_uri(string s, int pos, int sourceLength)</remarks>
        private static int Scan(string s, int pos, int sourceLength)
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
    }
}
