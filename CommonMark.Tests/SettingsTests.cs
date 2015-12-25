using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        [TestCategory("Inlines - Soft line break")]
        public void RenderSoftLineBreakAsLineBreak()
        {
            var settings = CommonMarkSettings.Default.Clone();
#pragma warning disable 0618
            settings.RenderSoftLineBreaksAsLineBreaks = true;
#pragma warning restore 0618
            Helpers.ExecuteTest("A\nB\nC", "<p>A<br />\nB<br />\nC</p>", settings);
        }
    }
}
