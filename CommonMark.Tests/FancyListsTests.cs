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
                        Features = Extension.FancyListsFeatures.All,
                        NumericListStyles = Extension.NumericListStyles.All,
                        AlphaListStyles = Extension.AlphaListStyles.All,
                        AdditiveListStyles = Extension.AdditiveListStyles.All,
                    }));
                    _fullSettings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerLatinListDisabledByDefault()
        {
            Helpers.ExecuteTest("a. foo\n", "<p>a. foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerLatinListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.LowerLatin)));
            Helpers.ExecuteTest("a. foo\n", "<p>a. foo</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerLatinListRequiresContent()
        {
            Helpers.ExecuteTest("a.\n", "<p>a.</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerLatinListNoSpace()
        {
            Helpers.ExecuteTest("b.ar\n", "<p>b.ar</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerLatinListStart26()
        {
            Helpers.ExecuteTest("z. foo\n", "<ol start=\"26\" type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerLatinListStart27()
        {
            Helpers.ExecuteTest("aa. foo\n", "<ol start=\"27\" type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListDisabledByDefault()
        {
            Helpers.ExecuteTest("A.  foo\n", "<p>A.  foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.UpperLatin)));
            Helpers.ExecuteTest("A.  foo\n", "<p>A.  foo</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListRequiresContent()
        {
            Helpers.ExecuteTest("A.  \n", "<p>A.</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListNoSpace()
        {
            Helpers.ExecuteTest("B.ar\n", "<p>B.ar</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListSingleSpace()
        {
            Helpers.ExecuteTest("Z. foo\n", "<p>Z. foo</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListDoubleSpace()
        {
            Helpers.ExecuteTest("F.  bar\n", "<ol start=\"6\" type=\"A\" style=\"list-style-type: upper-latin\">\n<li>bar</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinListStart27()
        {
            Helpers.ExecuteTest("AA.  foo\n", "<ol start=\"27\" type=\"A\" style=\"list-style-type: upper-latin\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperLatinLowerLatin()
        {
            Helpers.ExecuteTest("Aa.  bar\n", "<p>Aa.  bar</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListDisabledByDefault()
        {
            Helpers.ExecuteTest("i. foo\n", "<p>i. foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.LowerRoman)));
            Helpers.ExecuteTest("i. foo\n", "<ol start=\"9\" type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListNoSpace()
        {
            Helpers.ExecuteTest("i.fee\n", "<p>i.fee</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListStart1()
        {
            Helpers.ExecuteTest("i. fee\n", "<ol type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fee</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListStart2()
        {
            Helpers.ExecuteTest("ii. fi\n", "<ol start=\"2\" type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fi</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListStart4()
        {
            Helpers.ExecuteTest("iv. fo\n", "<ol start=\"4\" type=\"i\" style=\"list-style-type: lower-roman\">\n<li>fo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanListStart5()
        {
            Helpers.ExecuteTest("v. foo\n", "<ol start=\"5\" type=\"i\" style=\"list-style-type: lower-roman\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRomanInsideLowerLatinList()
        {
            Helpers.ExecuteTest("a. Apples\nb. Blackberries\nc. Cantaloops\nd. Durians", "<ol type=\"a\" style=\"list-style-type: lower-latin\">\n<li>Apples</li>\n<li>Blackberries</li>\n<li>Cantaloops</li>\n<li>Durians</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void TrailingLowerRomanInLowerLatinList()
        {
            Helpers.ExecuteTest("c. foo\nd. bar", "<ol start=\"3\" type=\"a\" style=\"list-style-type: lower-latin\">\n<li>foo</li>\n<li>bar</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRomanListDisabledByDefault()
        {
            Helpers.ExecuteTest("I.  foo\n", "<p>I.  foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRomanListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.UpperRoman)));
            Helpers.ExecuteTest("I.  foo\n", "<ol start=\"9\" type=\"A\" style=\"list-style-type: upper-latin\">\n<li>foo</li>\n</ol>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRomanListRequiresContent()
        {
            Helpers.ExecuteTest("MIC.", "<p>MIC.</p>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRomanListStart1984()
        {
            Helpers.ExecuteTest("MCMLXXXIV.  O.", "<ol start=\"1984\" type=\"I\" style=\"list-style-type: upper-roman\">\n<li>O.</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRomanInsideUpperLatinList()
        {
            Helpers.ExecuteTest("A.  Apples\nB.  Blackberries\nC.  Androids\nD.  Sheep", "<ol type=\"A\" style=\"list-style-type: upper-latin\">\n<li>Apples</li>\n<li>Blackberries</li>\n<li>Androids</li>\n<li>Sheep</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void TrailingUpperRomanInUpperLatinList()
        {
            Helpers.ExecuteTest("I.  foo\nJ.  bar", "<ol start=\"9\" type=\"A\" style=\"list-style-type: upper-latin\">\n<li>foo</li>\n<li>bar</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SharpListDisabledByDefault()
        {
            Helpers.ExecuteTest("#. foo", "<p>#. foo</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SharpListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.OrderedSharps)));
            Helpers.ExecuteTest("#. foo", "<p>#. foo</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void DecimalListEnabled()
        {
            Helpers.ExecuteTest("1. foo", "<ol type=\"1\">\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SharpListEnabled()
        {
            Helpers.ExecuteTest("#. foo", "<ol>\n<li>foo</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SharpListBeforeDecimalList()
        {
            Helpers.ExecuteTest("#. foo\n1. bar", "<ol>\n<li>foo</li>\n</ol>\n<ol type=\"1\">\n<li>bar</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SharpListAfterDecimalList()
        {
            Helpers.ExecuteTest("1. bar\n#. baz", "<ol type=\"1\">\n<li>bar</li>\n</ol>\n<ol>\n<li>baz</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void DiscListDisabledByDefault()
        {
            Helpers.ExecuteTest("● foo\n● bar", "<p>● foo\n● bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void DiscListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.Disc)));
            Helpers.ExecuteTest("● foo\n● bar", "<p>● foo\n● bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void DiscList1()
        {
            Helpers.ExecuteTest("● foo\n● bar", "<ul style=\"list-style-type: disc\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void CircleListDisabledByDefault()
        {
            Helpers.ExecuteTest("○ foo\n○ bar", "<p>○ foo\n○ bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void CircleListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.Circle)));
            Helpers.ExecuteTest("○ foo\n○ bar", "<p>○ foo\n○ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void CircleList1()
        {
            Helpers.ExecuteTest("○ foo\n○ bar", "<ul style=\"list-style-type: circle\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SquareListDisabledByDefault()
        {
            Helpers.ExecuteTest("■ foo\n■ bar", "<p>■ foo\n■ bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SquareListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.Square)));
            Helpers.ExecuteTest("■ foo\n■ bar", "<p>■ foo\n■ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void SquareList1()
        {
            Helpers.ExecuteTest("■ foo\n■ bar", "<ul style=\"list-style-type: square\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UnbulletedListDisabledByDefault()
        {
            Helpers.ExecuteTest("∙ foo\n∙ bar", "<p>∙ foo\n∙ bar</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UnbulletedListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(FancyListsFeatures.All & ~FancyListsFeatures.Unbulleted)));
            Helpers.ExecuteTest("∙ foo\n∙ bar", "<p>∙ foo\n∙ bar</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UnbulletedList1()
        {
            Helpers.ExecuteTest("∙ foo\n∙ bar", "<ul style=\"list-style-type: none\">\n<li>foo</li>\n<li>bar</li>\n</ul>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void ArabicIndicDisabledByDefault()
        {
            Helpers.ExecuteTest("٠.\n", "<p>٠.</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void ArabicIndicDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.ArabicIndic)));
            Helpers.ExecuteTest("٠.\n", "<p>٠.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void ArabicIndicStart1()
        {
            Helpers.ExecuteTest("١.\n", "<ol style=\"list-style-type: arabic-indic\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void ArabicIndicStart10()
        {
            Helpers.ExecuteTest("١٠.\n", "<ol start=\"10\" style=\"list-style-type: arabic-indic\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void PersianDisabledByDefault()
        {
            Helpers.ExecuteTest("۰.\n", "<p>۰.</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void PersianDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.Persian)));
            Helpers.ExecuteTest("۰.\n", "<p>۰.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void PersianAfterArabicIndic()
        {
            Helpers.ExecuteTest("١.\n۱.\n", "<ol style=\"list-style-type: arabic-indic\">\n<li></li>\n</ol>\n<ol style=\"list-style-type: persian\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void PersianStart1()
        {
            Helpers.ExecuteTest("۱.\n", "<ol style=\"list-style-type: persian\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void PersianStart10()
        {
            Helpers.ExecuteTest("۱۰.\n", "<ol start=\"10\" style=\"list-style-type: persian\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void FullWidthDecimalDisabledByDefault()
        {
            Helpers.ExecuteTest("０.\n", "<p>０.</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void FullWidthDecimalDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(NumericListStyles.All & ~NumericListStyles.FullWidthDecimal)));
            Helpers.ExecuteTest("０.\n", "<p>０.</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void FullWidthDecimalStart9()
        {
            Helpers.ExecuteTest("９.\n", "<ol start=\"9\" style=\"list-style-type: fullwidth-decimal\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void FullWidthDecimalStart10()
        {
            Helpers.ExecuteTest("１０.\n", "<ol start=\"10\" style=\"list-style-type: fullwidth-decimal\">\n<li></li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerGreekListDisabledByDefault()
        {
            Helpers.ExecuteTest("α) άλφα", "<p>α) άλφα</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerGreekListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AlphaListStyles.All & ~AlphaListStyles.LowerGreek)));
            Helpers.ExecuteTest("α. άλφα", "<p>α. άλφα</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerGreekListStart1()
        {
            Helpers.ExecuteTest("α) άλφα", "<ol style=\"list-style-type: lower-greek\">\n<li>άλφα</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerGreekListStart24()
        {
            Helpers.ExecuteTest("ω. ωμέγα", "<ol start=\"24\" style=\"list-style-type: lower-greek\">\n<li>ωμέγα</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerGreekListStart25()
        {
            Helpers.ExecuteTest("αα) δυο άλφα", " <ol start=\"25\" style=\"list-style-type: lower-greek\">\n<li>δυο άλφα</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRussianListDisabledByDefault()
        {
            Helpers.ExecuteTest("а) арбузы", "<p>а) арбузы</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRussianListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AlphaListStyles.All & ~AlphaListStyles.LowerRussian)));
            Helpers.ExecuteTest("а. арбузы", "<p>а. арбузы</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRussianListStart1()
        {
            Helpers.ExecuteTest("а) арбузы", "<ol style=\"list-style-type: lower-russian\">\n<li>арбузы</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRussianListStart28()
        {
            Helpers.ExecuteTest("я. яблоки", "<ol start=\"28\" style=\"list-style-type: lower-russian\">\n<li>яблоки</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void LowerRussianListStart29()
        {
            Helpers.ExecuteTest("аа) алыча", "<ol start=\"29\" style=\"list-style-type: lower-russian\">\n<li>алыча</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRussianListDisabledByDefault()
        {
            Helpers.ExecuteTest("А) арбузы", "<p>А) арбузы</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRussianListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AlphaListStyles.All & ~AlphaListStyles.UpperRussian)));
            Helpers.ExecuteTest("А.  Арбузы", "<p>А.  Арбузы</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRussianListStart1()
        {
            Helpers.ExecuteTest("А) Арбузы", "<ol style=\"list-style-type: upper-russian\">\n<li>Арбузы</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRussianListStart28()
        {
            Helpers.ExecuteTest("Я.  Яблоки", "<ol start=\"28\" style=\"list-style-type: upper-russian\">\n<li>Яблоки</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void UpperRussianListStart29()
        {
            Helpers.ExecuteTest("АА) Алыча", "<ol start=\"29\" style=\"list-style-type: upper-russian\">\n<li>Алыча</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HiraganaListDisabledByDefault()
        {
            Helpers.ExecuteTest("あ、 1", "<p>あ、 1</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HiraganaListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AlphaListStyles.All & ~AlphaListStyles.Hiragana)));
            Helpers.ExecuteTest("あ、 1", "<p>あ、 1</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HiraganaListStart1()
        {
            Helpers.ExecuteTest("あ、 1", "<ol style=\"list-style-type: hiragana\">\n<li>1</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HiraganaListStart46()
        {
            Helpers.ExecuteTest("ん、 46", "<ol start=\"46\" style=\"list-style-type: hiragana\">\n<li>46</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HiraganaListStart47()
        {
            Helpers.ExecuteTest("ああ、 47", "<ol start=\"47\" style=\"list-style-type: hiragana\">\n<li>47</li>\n</ol>", FullSettings);
        }


        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void KatakanaListDisabledByDefault()
        {
            Helpers.ExecuteTest("ア、 1", "<p>ア、 1</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void KatakanaListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AlphaListStyles.All & ~AlphaListStyles.Katakana)));
            Helpers.ExecuteTest("ア、 1", "<p>ア、 1</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void KatakanaListStart1()
        {
            Helpers.ExecuteTest("ア、 1", "<ol style=\"list-style-type: katakana\">\n<li>1</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void KatakanaListStart46()
        {
            Helpers.ExecuteTest("ン、 46", "<ol start=\"46\" style=\"list-style-type: katakana\">\n<li>46</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void KatakanaListStart47()
        {
            Helpers.ExecuteTest("アア、 47", "<ol start=\"47\" style=\"list-style-type: katakana\">\n<li>47</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListDisabledByDefault()
        {
            Helpers.ExecuteTest("יא. אללה הוא עכבר", "<p>יא. אללה הוא עכבר</p>", EmptySettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListDisabledAlone()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new FancyLists(s, new FancyListsSettings(AdditiveListStyles.None & ~AdditiveListStyles.Hebrew)));
            Helpers.ExecuteTest("יא. אללה הוא עכבר", "<p>יא. אללה הוא עכבר</p>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListStart1()
        {
            Helpers.ExecuteTest("א) תפוחים", "<ol style=\"list-style-type: hebrew\">\n<li>תפוחים</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListStart11()
        {
            Helpers.ExecuteTest("יא.  תמרים", "<ol start=\"11\" style=\"list-style-type: hebrew\">\n<li>תמרים</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListStart15()
        {
            Helpers.ExecuteTest("טו.  ויאמר אלהים יהי אור ויהי אור", "<ol start=\"15\" style=\"list-style-type: hebrew\">\n<li>ויאמר אלהים יהי אור ויהי אור</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListStart30()
        {
            Helpers.ExecuteTest("ל.  יש לו חום גבוה", "<ol start=\"30\" style=\"list-style-type: hebrew\">\n<li>יש לו חום גבוה</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListStart400()
        {
            Helpers.ExecuteTest("ת.  פ.", "<ol start=\"400\" style=\"list-style-type: hebrew\">\n<li>פ.</li>\n</ol>", FullSettings);
        }

        [TestMethod]
        [TestCategory("Container blocks - Fancy lists")]
        public void HebrewListRequiresContent()
        {
            Helpers.ExecuteTest("תא.", "<p>תא.</p>", FullSettings);
        }
    }
}
