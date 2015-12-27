using CommonMark.Syntax;
using System.Text;

namespace CommonMark.Formatters
{
    internal sealed class SyntaxFormatter : ISyntaxFormatter
    {
        private StringBuilder buffer;

        internal SyntaxFormatter()
        {
            this.buffer = new StringBuilder();
        }

        string ISyntaxFormatter.Format(string s)
        {
            return TextSyntaxFormatter.format_str(s, buffer);
        }

        string ISyntaxFormatter.Format(StringContent stringContent)
        {
            return ((ISyntaxFormatter)this).Format(stringContent.ToString(buffer));
        }
    }
}
