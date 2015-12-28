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
        [TestCategory("Container blocks - Loose Lists")]
        public void LooseListsDisabledByDefault()
        {
            Helpers.ExecuteTest("- baz\n+ foo\n\n+ bar", "<ul>\n<li>baz</li>\n</ul>\n<ul>\n<li><p>foo</p></li>\n<li><p>bar</p></li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - Loose Lists")]
        public void LooseLists()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new Extension.LooseLists(new Extension.LooseListsSettings(false)));
            Helpers.ExecuteTest("- baz\n+ foo\n\n+ bar", "<ul>\n<li><p>baz</p></li>\n</ul>\n<ul>\n<li><p>foo</p></li>\n<li><p>bar</p></li>\n</ul>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - Loose Lists")]
        public void TightLists()
        {
            var s = CommonMarkSettings.Default.Clone();
            s.Extensions.Register(new Extension.LooseLists(new Extension.LooseListsSettings(true)));
            Helpers.ExecuteTest("- baz\n+ foo\n\n+ bar", "<ul>\n<li>baz</li>\n</ul>\n<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>", s);
        }
    }
}
