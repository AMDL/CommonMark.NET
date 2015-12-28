using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class MathTests
    {
        private static CommonMarkSettings _settings;
        private static CommonMarkSettings Settings
        {
            get
            {
                var s = _settings;
                if (s == null)
                {
                    s = CommonMarkSettings.Default.Clone();
                    s.Extensions.Register(new Extension.MathDollars());
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathDisabledByDefault()
        {
            Helpers.ExecuteTest("foo $bar$ asd", "<p>foo $bar$ asd</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathEscapeOpener()
        {
            Helpers.ExecuteTest("foo \\$bar$ asd", "<p>foo $bar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathEscapeCloser()
        {
            Helpers.ExecuteTest("foo $bar\\$ asd", "<p>foo $bar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingSpace()
        {
            Helpers.ExecuteTest("foo $ bar$ asd", "<p>foo $ bar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathTrailingSpace()
        {
            Helpers.ExecuteTest("foo $bar $ asd", "<p>foo $bar $ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingAndTrailingSpaces()
        {
            Helpers.ExecuteTest("foo $ bar $ asd", "<p>foo $ bar $ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingTab()
        {
            Helpers.ExecuteTest("foo $\tbar$ asd", "<p>foo $\tbar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathTrailingTab()
        {
            Helpers.ExecuteTest("foo $bar\t$ asd", "<p>foo $bar\t$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingAndTrailingTabs()
        {
            Helpers.ExecuteTest("foo $\tbar\t$ asd", "<p>foo $\tbar\t$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathZeroAfter()
        {
            Helpers.ExecuteTest("foo $bar$0 asd", "<p>foo $bar$0 asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathOneAfter()
        {
            Helpers.ExecuteTest("foo $bar$1 asd", "<p>foo $bar$1 asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathNineAfter()
        {
            Helpers.ExecuteTest("foo $bar$9 asd", "<p>foo $bar$9 asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingZero()
        {
            Helpers.ExecuteTest("foo $0bar$ asd", "<p>foo <span class=\"math\">0bar</span> asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingOne()
        {
            Helpers.ExecuteTest("foo $1bar$ asd", "<p>foo <span class=\"math\">1bar</span> asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingNine()
        {
            Helpers.ExecuteTest("foo $9bar$ asd", "<p>foo <span class=\"math\">9bar</span> asd</p>", Settings);
        }
    }
}
