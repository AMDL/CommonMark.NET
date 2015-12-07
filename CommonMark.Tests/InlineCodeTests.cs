using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMark.Tests
{
    [TestClass]
    public class InlineCodeTests
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
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.EmphasisInInlineCode;
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Inline Code - Emphasis and strong emphasis")]
        public void EmphasisInInlineCodeDisabledByDefault()
        {
            Helpers.ExecuteTest("`_foo_`", "<p><code>_foo_</code></p>");
        }

        [TestMethod]
        [TestCategory("Inline Code - Emphasis and strong emphasis")]
        public void EmphasisInInlineCodeDisabledInPost()
        {
            Helpers.ExecuteTest("foo `_ba[r_`", "<p>foo <code>_ba[r_</code></p>");
        }

        [TestMethod]
        [TestCategory("Inline Code - Emphasis and strong emphasis")]
        public void EmphasisInInlineCodeNestedBacktick()
        {
            Helpers.ExecuteTest("`` ` ``", "<p><code>`</code></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inline Code - Emphasis and strong emphasis")]
        public void EmphasisInInlineCodeExample()
        {
            Helpers.ExecuteTest("`_foo_`", "<p><code><em>foo</em></code></p>", Settings);
        }

        [TestMethod]
        [TestCategory("Inline Code - Emphasis and strong emphasis")]
        public void EmphasisInInlineCodeExample2()
        {
            Helpers.ExecuteTest("`*foo*`", "<p><code><em>foo</em></code></p>", Settings);
        }
    }
}
