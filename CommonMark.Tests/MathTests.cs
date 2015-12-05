using System;
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
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.MathDollar;
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
        public void MathLeadingZero()
        {
            Helpers.ExecuteTest("foo $0bar$ asd", "<p>foo $0bar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingOne()
        {
            Helpers.ExecuteTest("foo $1bar$ asd", "<p>foo $1bar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathLeadingNine()
        {
            Helpers.ExecuteTest("foo $9bar$ asd", "<p>foo $9bar$ asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathTrailingZero()
        {
            Helpers.ExecuteTest("foo $bar0$ asd", "<p>foo <span class=\"math\">bar0</span> asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathTrailingOne()
        {
            Helpers.ExecuteTest("foo $bar1$ asd", "<p>foo <span class=\"math\">bar1</span> asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathTrailingNine()
        {
            Helpers.ExecuteTest("foo $bar9$ asd", "<p>foo <span class=\"math\">bar9</span> asd</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample1()
        {
            Helpers.ExecuteTest("foo $bar$", "<p>foo <span class=\"math\">bar</span></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample2()
        {
            Helpers.ExecuteTest("foo $$bar$", "<p>foo $<span class=\"math\">bar</span></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample3()
        {
            Helpers.ExecuteTest("foo $$bar$ asd$", "<p>foo <span class=\"math\"><span class=\"math\">bar</span> asd</span></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample4()
        {
            Helpers.ExecuteTest("foo $*bar$*", "<p>foo <span class=\"math\">*bar</span>*</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample5()
        {
            Helpers.ExecuteTest("foo *$bar$*", "<p>foo <em><span class=\"math\">bar</span></em></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample6()
        {
            Helpers.ExecuteTest("foo **$bar**$", "<p>foo <strong>$bar</strong>$</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample7()
        {
            Helpers.ExecuteTest("$bar$$", "<p><span class=\"math\">bar</span>$</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample10a()
        {
            // '[' char in the middle will delay the $ match to the post-process phase.
            Helpers.ExecuteTest("foo $$ba[r$", "<p>foo $<span class=\"math\">ba[r</span></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample10b()
        {
            // '[' char in the middle will delay the $ match to the post-process phase.
            Helpers.ExecuteTest("foo $ba[r$$", "<p>foo <span class=\"math\">ba[r</span>$</p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Math")]
        public void MathExample10d()
        {
            // '[' char in the middle will delay the $ match to the post-process phase.
            Helpers.ExecuteTest("$$[foo$ bar", "<p>$<span class=\"math\">[foo</span> bar</p>", Settings);
        }
    }
}
