using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class CustomContainerTests
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
                    s.AdditionalFeatures |= CommonMarkAdditionalFeatures.CustomContainers;
                    _settings = s;
                }
                return s;
            }
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainersDisabledByDefault()
        {
            Helpers.ExecuteTest(":::foo\nbar\n:::", "<p>:::foo\nbar\n:::</p>");
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainersString()
        {
            Helpers.ExecuteTest(":::foo\nbar\n:::", "<div class=\"foo\">bar</div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainersEmphasis()
        {
            Helpers.ExecuteTest(":::foo\n_bar_\n:::", "<div class=\"foo\"><em>bar</em></div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainerExample077()
        {
            Helpers.ExecuteTest(":::\n<\n >\n:::", "<div class=\"\">&lt;\n&gt;</div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainerExample079()
        {
            Helpers.ExecuteTest(":::\naaa\n~~~\n:::", "<div class=\"\">aaa\n~~~</div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainerExample080()
        {
            Helpers.ExecuteTest(":::\naaa\n```\n:::", "<div class=\"\">aaa\n```</div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainerExample081()
        {
            Helpers.ExecuteTest("::::\naaa\n:::\n::::::", "<div class=\"\">aaa\n:::</div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainerExample082()
        {
            Helpers.ExecuteTest("::::\naaa\n:::\n:::::", "<div class=\"\">aaa\n:::</div>", Settings);
        }

        [TestMethod]
        [TestCategory("Leaf blocks - Custom containers")]
        public void CustomContainerExample083()
        {
            Helpers.ExecuteTest(":::", "<div class=\"\"></div>", Settings);
        }
    }
}
