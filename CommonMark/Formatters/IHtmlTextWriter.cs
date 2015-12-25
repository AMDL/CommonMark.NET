using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// HTML writer interface.
    /// </summary>
    public interface IHtmlTextWriter
    {
        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        void WriteConstant(char[] value);

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        void WriteConstant(char[] value, int startIndex, int length);

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        void WriteConstant(string value);

        /// <summary>
        /// Writes a value that is known not to contain any newlines.
        /// </summary>
        void WriteLineConstant(string value);

        /// <summary>
        /// Writes a value.
        /// </summary>
        void Write(char value);

        /// <summary>
        /// Writes a value.
        /// </summary>
        void Write(StringPart value);

        /// <summary>
        /// Writes a value.
        /// </summary>
        void Write(char[] value, int index, int count);

        /// <summary>
        /// Writes a value.
        /// </summary>
        void Write(StringContent stringContent);

        /// <summary>
        /// Writes a newline.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a value.
        /// </summary>
        void WriteLine(char value);

        /// <summary>
        /// Adds a newline if the writer does not currently end with a newline.
        /// </summary>
        void EnsureLine();

        /// <summary>
        /// Writes the position of a block element.
        /// </summary>
        void WritePosition(Block block);

        /// <summary>
        /// Writes the position of an inline element.
        /// </summary>
        void WritePosition(Inline inline);

        /// <summary>
        /// Encodes the given text with HTML encoding (ampersand-encoding) and writes the result.
        /// </summary>
        void WriteEncodedHtml(StringPart part);

        /// <summary>
        /// Encodes the given text with HTML encoding (ampersand-encoding) and writes the result.
        /// </summary>
        void WriteEncodedHtml(StringContent stringContent);

        /// <summary>
        /// Encodes the given text with URL encoding (percent-encoding) and writes the result.
        /// </summary>
        void WriteEncodedUrl(string url);
    }
}
