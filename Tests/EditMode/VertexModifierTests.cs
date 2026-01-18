using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using NovelUIKit.Effects.VertexModifiers;

namespace NovelUIKit.Tests.EditMode
{
    public class VertexModifierTests
    {
        private static TextMeshPro CreateText(string text)
        {
            GameObject gameObject = new GameObject("TMP_Test");
            TextMeshPro tmp = gameObject.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.ForceMeshUpdate();
            return tmp;
        }

        [Test]
        public void ShakeModifier_OffsetsVertices()
        {
            TextMeshPro tmp = CreateText("Shake");
            try
            {
                TMP_TextInfo textInfo = tmp.textInfo;
                Vector3[] original = textInfo.meshInfo[0].vertices.ToArray();

                ShakeModifier modifier = new ShakeModifier { Amplitude = 3f, Frequency = 12f };
                modifier.ModifyVertices(textInfo, 0, textInfo.characterCount - 1, 0f);

                Vector3[] modified = textInfo.meshInfo[0].vertices;
                Assert.IsTrue(original.Where((t, i) => t != modified[i]).Any(), "Expected vertices to be offset.");
            }
            finally
            {
                Object.DestroyImmediate(tmp.gameObject);
            }
        }

        [Test]
        public void GlyphCorruptionModifier_ReplacesCharacters()
        {
            TextMeshPro tmp = CreateText("Glyph");
            try
            {
                TMP_TextInfo textInfo = tmp.textInfo;
                GlyphCorruptionModifier modifier = new GlyphCorruptionModifier { CorruptionSpeed = 10f };

                modifier.ModifyVertices(textInfo, 0, textInfo.characterCount - 1, 0f);

                char[] corruptionSet =
                {
                    'ᚠ', 'ᚢ', 'ᚦ', 'ᚨ', 'ᚱ', 'ᚲ', 'ᚷ', 'ᚹ', 'ᚺ', 'ᚾ',
                    'ᛁ', 'ᛃ', 'ᛇ', 'ᛈ', 'ᛉ', 'ᛋ', 'ᛏ', 'ᛒ', 'ᛖ', 'ᛗ',
                    'ᛚ', 'ᛜ', 'ᛞ', 'ᛟ', 'ᛝ', 'ᛡ', 'ᛠ', '⸸', '☠', '♆',
                    '☾', '☿', '♄', '♃', '♀', '♂', '⚶', '⚸'
                };

                Assert.IsTrue(tmp.text.Any(c => corruptionSet.Contains(c)), "Expected corrupted glyphs in text.");
            }
            finally
            {
                Object.DestroyImmediate(tmp.gameObject);
            }
        }

        [TestCase(DistortionMode.Crush)]
        [TestCase(DistortionMode.Melt)]
        [TestCase(DistortionMode.Shatter)]
        [TestCase(DistortionMode.Stretch)]
        public void DistortionModifier_OffsetsVertices(DistortionMode mode)
        {
            TextMeshPro tmp = CreateText("Warp");
            try
            {
                TMP_TextInfo textInfo = tmp.textInfo;
                Vector3[] original = textInfo.meshInfo[0].vertices.ToArray();

                DistortionModifier modifier = new DistortionModifier
                {
                    Mode = mode,
                    Amplitude = 3f,
                    Frequency = 6f
                };

                modifier.ModifyVertices(textInfo, 0, textInfo.characterCount - 1, 0f);

                Vector3[] modified = textInfo.meshInfo[0].vertices;
                Assert.IsTrue(original.Where((t, i) => t != modified[i]).Any(), "Expected vertices to be distorted.");
            }
            finally
            {
                Object.DestroyImmediate(tmp.gameObject);
            }
        }
    }
}
