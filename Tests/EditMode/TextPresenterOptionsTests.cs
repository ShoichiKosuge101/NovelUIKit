using System.Collections.Generic;
using NovelUIKit.Runtime.Effects;
using NovelUIKit.Runtime.Presenters;
using NUnit.Framework;

namespace NovelUIKit.Tests.EditMode
{
    public sealed class TextPresenterOptionsTests
    {
        private sealed class DummyModifier : IVertexModifier
        {
            public int CallCount { get; private set; }

            public void Apply(TMPro.TMP_Text text, int startIndex, int endIndex, float timeSeconds)
            {
                CallCount++;
            }
        }

        [Test]
        public void DefaultOptionsProvideEmptyEffectRanges()
        {
            var options = TextPresenterOptions.Default;

            Assert.That(options.EffectRanges, Is.Not.Null);
            Assert.That(options.EffectRanges.Count, Is.EqualTo(0));
        }

        [Test]
        public void TextEffectRangeStoresModifiers()
        {
            var modifier = new DummyModifier();
            var range = new TextEffectRange(0, 3, new List<IVertexModifier> { modifier });

            Assert.That(range.StartIndex, Is.EqualTo(0));
            Assert.That(range.Length, Is.EqualTo(3));
            Assert.That(range.Modifiers, Has.Count.EqualTo(1));
        }
    }
}
