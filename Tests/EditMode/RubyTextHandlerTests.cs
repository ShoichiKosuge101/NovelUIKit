using NovelUIKit.Runtime.TextPresenter;
using NUnit.Framework;

namespace NovelUIKit.Tests.EditMode
{
    public sealed class RubyTextHandlerTests
    {
        [Test]
        public void ParseRubyTagExtractsAnnotations()
        {
            var handler = new RubyTextHandler();
            var result = handler.Parse("これは<ruby>漢字<rt>かんじ</rt></ruby>です");

            Assert.That(result.OutputText, Is.EqualTo("これは漢字です"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            Assert.That(result.Annotations[0].BaseStartIndex, Is.EqualTo(3));
            Assert.That(result.Annotations[0].BaseLength, Is.EqualTo(2));
            Assert.That(result.Annotations[0].RubyText, Is.EqualTo("かんじ"));
        }

        [Test]
        public void ParseRubyTagPreservesRichTextOutsideRuby()
        {
            var handler = new RubyTextHandler();
            var result = handler.Parse("<color=red>赤</color><ruby>青<rt>あお</rt></ruby>");

            Assert.That(result.OutputText, Is.EqualTo("<color=red>赤</color>青"));
            Assert.That(result.Annotations[0].BaseStartIndex, Is.EqualTo(1));
            Assert.That(result.Annotations[0].BaseLength, Is.EqualTo(1));
            Assert.That(result.Annotations[0].RubyText, Is.EqualTo("あお"));
        }

        [Test]
        public void VisibleIndexIgnoresRubyTextAndRichTextTags()
        {
            var handler = new RubyTextHandler();
            var text = "A<ruby>漢字<rt>かんじ</rt></ruby><b>B</b>";

            var rawIndexForKanji = text.IndexOf("漢字", System.StringComparison.Ordinal);
            var rawIndexForBoldB = text.IndexOf('B');

            Assert.That(handler.GetVisibleIndexAtRawPosition(text, rawIndexForKanji), Is.EqualTo(1));
            Assert.That(handler.GetVisibleIndexAtRawPosition(text, rawIndexForBoldB), Is.EqualTo(3));
        }
    }
}
