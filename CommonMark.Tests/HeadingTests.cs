using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class HeadingTests
    {
        private static CommonMarkSettings _headerDotsSettings;
        private static CommonMarkSettings HeaderDotsSettings
        {
            get
            {
                var s = _headerDotsSettings;
                if (s == null)
                {
                    s = CommonMarkSettings.Default.Clone();
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.HeaderDots;
                    _headerDotsSettings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Leaf blocks - ATX headers")]
        public void HeadingsAndHorizontalRulers()
        {
            // see https://github.com/Knagis/CommonMark.NET/issues/60
            Helpers.ExecuteTest("##### A\n---\n\n##### B\n---\n\n##### C\n---", "<h5>A</h5>\n<hr />\n<h5>B</h5>\n<hr />\n<h5>C</h5>\n<hr />\n");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsDisabledByDefault()
        {
            Helpers.ExecuteTest("foo\n..\n", "<p>foo\n..</p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsDisabledSingleDot()
        {
            Helpers.ExecuteTest("foo\n.\n", "<p>foo\n.</p>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample045()
        {
            Helpers.ExecuteTest("Foo *bar*\n.........\n\nFoo *bar*\n---------", "<h3>Foo <em>bar</em></h3>\n<h2>Foo <em>bar</em></h2>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample046()
        {
            Helpers.ExecuteTest("Foo\n.........................\n\nFoo\n-", "<h3>Foo</h3>\n<h2>Foo</h2>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample047()
        {
            Helpers.ExecuteTest("   Foo\n...\n\n  Foo\n.....\n\n  Foo\n  ---", "<h3>Foo</h3>\n<h3>Foo</h3>\n<h2>Foo</h2>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample048()
        {
            Helpers.ExecuteTest("    Foo\n    ...\n\n    Foo\n---", "<pre><code>Foo\n...\n\nFoo\n</code></pre>\n<hr />", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample049()
        {
            Helpers.ExecuteTest("Foo\n   ....      ", "<h3>Foo</h3>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample050()
        {
            Helpers.ExecuteTest("Foo\n    ...", "<p>Foo\n...</p>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample051()
        {
            Helpers.ExecuteTest("Foo\n. .\n\nFoo\n... .", "<p>Foo\n. .</p>\n<p>Foo\n... .</p>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample052()
        {
            Helpers.ExecuteTest("Foo  \n.....", "<h3>Foo</h3>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample053()
        {
            Helpers.ExecuteTest("Foo\\\n....", "<h3>Foo\\</h3>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample054()
        {
            Helpers.ExecuteTest("`Foo\n....\n`\n\n<a title=\"a lot\n...\nof dots\"/>", "<h3>`Foo</h3>\n<p>`</p>\n<h3>&lt;a title=&quot;a lot</h3>\n<p>of dots&quot;/&gt;</p>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample055()
        {
            Helpers.ExecuteTest("> Foo\n...", "<blockquote>\n<p>Foo\n...</p>\n</blockquote>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample056()
        {
            Helpers.ExecuteTest("- Foo\n...", "<ul>\n<li>Foo\n...</li>\n</ul>", HeaderDotsSettings);
        }

        //No Example057 for HeaderDot

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample058()
        {
            Helpers.ExecuteTest("---\nFoo\n...\nBar\n...\nBaz", "<hr />\n<h3>Foo</h3>\n<h3>Bar</h3>\n<p>Baz</p>", HeaderDotsSettings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Setext headers")]
        public void HeaderDotsExample059()
        {
            Helpers.ExecuteTest("....", "<p>....</p>", HeaderDotsSettings);
        }
    }
}
