using CommonMark.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class FancyListsTests
    {
        private static CommonMarkSettings _emptySettings;
        private static CommonMarkSettings EmptySettings
        {
            get
            {
                var s = _emptySettings;
                if (s == null)
                {
                    s = CommonMarkSettings.Default.Clone();
                    s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
                    _emptySettings = s;
                }
                return s;
            }
        }

        private static CommonMarkSettings _fullSettings;
        private static CommonMarkSettings FullSettings
        {
            get
            {
                var s = _fullSettings;
                if (s == null)
                {
                    s = CommonMarkSettings.Default.Clone();
                    s.Extensions.Register(new FancyLists(s, new FancyListsSettings
                    {
                        StandardListStyles = Extension.StandardListStyles.All,
                        BulletListStyles = Extension.BulletListStyles.All,
                        NumericListStyles = Extension.NumericListStyles.All,
                        AdditiveListStyles = Extension.AdditiveListStyles.All,
                    }));
                    _fullSettings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListDisabledByDefault()
        {
            Helpers.ExecuteTest("a. foo\n", "<p>a. foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.LowerLatin)));
            Helpers.ExecuteTest("a. foo\n", "<p>a. foo</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListRequiresContent()
        {
            Helpers.ExecuteTest("a.\n", "<p>a.</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListNoSpace()
        {
            Helpers.ExecuteTest("b.ar\n", "<p>b.ar</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListStart26()
        {
            Helpers.ExecuteTest("z. foo\n", "<ol start=\"26\" type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListDisabledByDefault()
        {
            Helpers.ExecuteTest("A.  foo\n", "<p>A.  foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.UpperLatin)));
            Helpers.ExecuteTest("A.  foo\n", "<p>A.  foo</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListRequiresContent()
        {
            Helpers.ExecuteTest("A.  \n", "<p>A.</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListNoSpace()
        {
            Helpers.ExecuteTest("B.ar\n", "<p>B.ar</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListSingleSpace()
        {
            Helpers.ExecuteTest("Z. foo\n", "<p>Z. foo</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListDoubleSpace()
        {
            Helpers.ExecuteTest("F.  bar\n", "<ol start=\"6\" type=\"A\" style=\"list-style-type: upper-latin\">\n<li>bar</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListDisabledByDefault()
        {
            Helpers.ExecuteTest("i. foo\n", "<p>i. foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.LowerRoman)));
            Helpers.ExecuteTest("i. foo\n", "<ol start=\"9\" type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListNoSpace()
        {
            Helpers.ExecuteTest("i.fee\n", "<p>i.fee</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListStart1()
        {
            Helpers.ExecuteTest("i. fee\n", "<ol type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fee</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListStart2()
        {
            Helpers.ExecuteTest("ii. fi\n", "<ol start=\"2\" type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fi</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListStart4()
        {
            Helpers.ExecuteTest("iv. fo\n", "<ol start=\"4\" type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListStart5()
        {
            Helpers.ExecuteTest("v. foo\n", "<ol start=\"5\" type=\"i\" style=\"list-style-type: lower-roman\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperRomanListDisabledByDefault()
        {
            Helpers.ExecuteTest("I.  foo\n", "<p>I.  foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperRomanListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.UpperRoman)));
            Helpers.ExecuteTest("I.  foo\n", "<ol start=\"9\" type=\"A\" style=\"list-style-type: upper-latin\">\n<li>foo</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperRomanListRequiresContent()
        {
            Helpers.ExecuteTest("MIC.", "<p>MIC.</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperRomanListStart1984()
        {
            Helpers.ExecuteTest("MCMLXXXIV.  O.", "<ol start=\"1984\" type=\"I\" style=\"list-style-type: upper-roman\">\n<li>O.</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void DiscListDisabledByDefault()
        {
            Helpers.ExecuteTest("● foo\n● bar", "<p>● foo\n● bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void DiscListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.All & ~BulletListStyles.Disc)));
            Helpers.ExecuteTest("● foo\n● bar", "<p>● foo\n● bar</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void DiscList1()
        {
            Helpers.ExecuteTest("● foo\n● bar", "<ul style=\"list-style-type: disc\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void CircleListDisabledByDefault()
        {
            Helpers.ExecuteTest("○ foo\n○ bar", "<p>○ foo\n○ bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void CircleListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.All & ~BulletListStyles.Circle)));
            Helpers.ExecuteTest("○ foo\n○ bar", "<p>○ foo\n○ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void CircleList1()
        {
            Helpers.ExecuteTest("○ foo\n○ bar", "<ul style=\"list-style-type: circle\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void SquareListDisabledByDefault()
        {
            Helpers.ExecuteTest("■ foo\n■ bar", "<p>■ foo\n■ bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void SquareListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.All & ~BulletListStyles.Square)));
            Helpers.ExecuteTest("■ foo\n■ bar", "<p>■ foo\n■ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void SquareList1()
        {
            Helpers.ExecuteTest("■ foo\n■ bar", "<ul style=\"list-style-type: square\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void ArabicIndicDisabledByDefault()
        {
            Helpers.ExecuteTest("٠.\n", "<p>٠.</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void ArabicIndicDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.ArabicIndic)));
            Helpers.ExecuteTest("٠.\n", "<p>٠.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void ArabicIndicStart1()
        {
            Helpers.ExecuteTest("١.\n", "<ol style=\"list-style-type: arabic-indic\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianDisabledByDefault()
        {
            Helpers.ExecuteTest("۰.\n", "<p>۰.</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.Persian)));
            Helpers.ExecuteTest("۰.\n", "<p>۰.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianAfterArabicIndic()
        {
            Helpers.ExecuteTest("١.\n۱.\n", "<ol style=\"list-style-type: arabic-indic\">\n<li></li>\n</ol>\n<ol style=\"list-style-type: persian\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianStart1()
        {
            Helpers.ExecuteTest("۱.\n", "<ol style=\"list-style-type: persian\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void FullWidthDecimalDisabledByDefault()
        {
            Helpers.ExecuteTest("０.\n", "<p>０.</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void FullWidthDecimalDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.FullWidthDecimal)));
            Helpers.ExecuteTest("０.\n", "<p>０.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void FullWidthDecimalStart9()
        {
            Helpers.ExecuteTest("９.\n", "<ol start=\"9\" style=\"list-style-type: fullwidth-decimal\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListDisabledByDefault()
        {
            Helpers.ExecuteTest("יא. אללה הוא עכבר", "<p>יא. אללה הוא עכבר</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AdditiveListStyles.None & ~AdditiveListStyles.Hebrew)));
            Helpers.ExecuteTest("יא. אללה הוא עכבר", "<p>יא. אללה הוא עכבר</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListStart11()
        {
            Helpers.ExecuteTest("יא.  אללה הוא עכבר", "<ol start=\"11\" style=\"list-style-type: hebrew\">\n<li>אללה הוא עכבר</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListStart15()
        {
            Helpers.ExecuteTest("טו.  ויאמר אלהים יהי אור ויהי אור", "<ol start=\"15\" style=\"list-style-type: hebrew\">\n<li>ויאמר אלהים יהי אור ויהי אור</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListStart30()
        {
            Helpers.ExecuteTest("ל.  יש לו חום גבוה", "<ol start=\"30\" style=\"list-style-type: hebrew\">\n<li>יש לו חום גבוה</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListStart400()
        {
            Helpers.ExecuteTest("ת.  פ.", "<ol start=\"400\" style=\"list-style-type: hebrew\">\n<li>פ.</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListRequiresContent()
        {
            Helpers.ExecuteTest("תא.", "<p>תא.</p>", FullSettings);
        }
    }
}
