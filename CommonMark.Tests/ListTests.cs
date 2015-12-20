using CommonMark.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommonMark.Tests
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        public void Example210WithPositionTracking()
        {
            // Example 210 handles case that relies on Block.SourcePosition property (everything else just sets it)

            var s = CommonMarkSettings.Default.Clone();
            s.TrackSourcePosition = true;

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 210, "Container blocks - Lists");
            Helpers.ExecuteTest("* a\n*\n\n* c", @"<ul␣data-sourcepos=""0-10"">
<li␣data-sourcepos=""0-4"">
<p␣data-sourcepos=""2-4""><span␣data-sourcepos=""2-3"">a</span></p>
</li>
<li␣data-sourcepos=""4-6""></li>
<li␣data-sourcepos=""7-10"">
<p␣data-sourcepos=""9-10""><span␣data-sourcepos=""9-10"">c</span></p>
</li>
</ul>
", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UnicodeBulletEscape()
        {
            Helpers.ExecuteTest("\\• foo\n\n\\* bar", "<p>• foo</p>\n<p>* bar</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UnicodeBulletList()
        {
            Helpers.ExecuteTest("• foo\n• bar", "<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void EmptyList1()
        {
            Helpers.ExecuteTest("1.\n2.", "<ol>\n<li></li>\n<li></li>\n</ol>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void EmptyList2()
        {
            Helpers.ExecuteTest("+\n+", "<ul>\n<li></li>\n<li></li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("a.\n", "<p>a.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.LowerLatin)));
            Helpers.ExecuteTest("a.\n", "<p>a.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListEmpty()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerLatin)));
            Helpers.ExecuteTest("a.\n", "<ol type=\"a\" style=\"list-style-type: lower-latin\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListNoSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerLatin)));
            Helpers.ExecuteTest("b.ar\n", "<p>b.ar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListSingleSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerLatin)));
            Helpers.ExecuteTest("z. foo\n", "<ol type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerLatinListDoubleSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerLatin)));
            Helpers.ExecuteTest("f.  bar\n", "<ol type=\"a\" style=\"list-style-type: lower-latin\">\n<li>bar</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("A.\n", "<p>A.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.UpperLatin)));
            Helpers.ExecuteTest("A.\n", "<p>A.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListEmpty()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.UpperLatin)));
            Helpers.ExecuteTest("A.\n", "<ol type=\"A\" style=\"list-style-type: upper-latin\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListNoSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.UpperLatin)));
            Helpers.ExecuteTest("B.ar\n", "<p>B.ar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListSingleSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.UpperLatin)));
            Helpers.ExecuteTest("Z. foo\n", "<p>Z. foo</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperLatinListDoubleSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.UpperLatin)));
            Helpers.ExecuteTest("F.  bar\n", "<ol type=\"A\" style=\"list-style-type: upper-latin\">\n<li>bar</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("i.\n", "<p>i.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.LowerRoman)));
            Helpers.ExecuteTest("i.\n", "<ol type=\"a\" style=\"list-style-type: lower-latin\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListNoSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerRoman)));
            Helpers.ExecuteTest("i.fee\n", "<p>i.fee</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListSingleSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerRoman)));
            Helpers.ExecuteTest("i. fee\n", "<ol type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fee</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void LowerRomanListDoubleSpace()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.LowerRoman)));
            Helpers.ExecuteTest("v.  fi\n", "<ol type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fi</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperRomanListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("I.\n", "<p>I.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UpperRomanListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(StandardListStyles.All & ~StandardListStyles.UpperRoman)));
            Helpers.ExecuteTest("I.\n", "<ol type=\"A\" style=\"list-style-type: upper-latin\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void DiscListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("● foo\n● bar", "<p>● foo\n● bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void DiscListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.All & ~BulletListStyles.Disc)));
            Helpers.ExecuteTest("● foo\n● bar", "<p>● foo\n● bar</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void DiscList1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.Disc)));
            Helpers.ExecuteTest("● foo\n● bar", "<ul style=\"list-style-type: disc\">\n<li>foo</li>\n<li>bar</li>\n</ul>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void CircleListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("○ foo\n○ bar", "<p>○ foo\n○ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void CircleListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.All & ~BulletListStyles.Circle)));
            Helpers.ExecuteTest("○ foo\n○ bar", "<p>○ foo\n○ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void CircleList1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.Circle)));
            Helpers.ExecuteTest("○ foo\n○ bar", "<ul style=\"list-style-type: circle\">\n<li>foo</li>\n<li>bar</li>\n</ul>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void SquareListDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("■ foo\n■ bar", "<p>■ foo\n■ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void SquareListDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.All & ~BulletListStyles.Square)));
            Helpers.ExecuteTest("■ foo\n■ bar", "<p>■ foo\n■ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void SquareList1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(BulletListStyles.Square)));
            Helpers.ExecuteTest("■ foo\n■ bar", "<ul style=\"list-style-type: square\">\n<li>foo</li>\n<li>bar</li>\n</ul>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void ArabicIndicDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("٠.\n", "<p>٠.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void ArabicIndicDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.ArabicIndic)));
            Helpers.ExecuteTest("٠.\n", "<p>٠.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void ArabicIndicStart1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.ArabicIndic)));
            Helpers.ExecuteTest("١.\n", "<ol style=\"list-style-type: arabic-indic\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("۰.\n", "<p>۰.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.Persian)));
            Helpers.ExecuteTest("۰.\n", "<p>۰.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void PersianStart1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.Persian)));
            Helpers.ExecuteTest("۱.\n", "<ol style=\"list-style-type: persian\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void FullwidthDecimalDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("０.\n", "<p>０.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void FullwidthDecimalDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.FullwidthDecimal)));
            Helpers.ExecuteTest("０.\n", "<p>０.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void FullwidthDecimalStart1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.FullwidthDecimal)));
            Helpers.ExecuteTest("９.\n", "<ol style=\"list-style-type: fullwidth-decimal\">\n<li></li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewDisabledByDefault()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings()));
            Helpers.ExecuteTest("יא. אללה הוא עכבר", "<p>יא. אללה הוא עכבר</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewDisabledByDefault2()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AdditiveListStyles.None & ~AdditiveListStyles.Hebrew)));
            Helpers.ExecuteTest("יא. אללה הוא עכבר", "<p>יא. אללה הוא עכבר</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewList1()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AdditiveListStyles.Hebrew)));
            Helpers.ExecuteTest("יא.  אללה הוא עכבר", "<ol style=\"list-style-type: hebrew\">\n<li>אללה הוא עכבר</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void HebrewListStart15()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AdditiveListStyles.Hebrew)));
            Helpers.ExecuteTest("טו.  ויאמר אלהים יהי אור ויהי אור", "<ol style=\"list-style-type: hebrew\">\n<li>ויאמר אלהים יהי אור ויהי אור</li>\n</ol>", s);
        }
    }
}
