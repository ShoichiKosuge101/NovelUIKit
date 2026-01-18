using TMPro;
using UnityEngine;

namespace NovelUIKit.Effects.VertexModifiers
{
    public sealed class GlyphCorruptionModifier : IVertexModifier
    {
        private static readonly char[] CorruptionGlyphs =
        {
            'ᚠ', 'ᚢ', 'ᚦ', 'ᚨ', 'ᚱ', 'ᚲ', 'ᚷ', 'ᚹ', 'ᚺ', 'ᚾ',
            'ᛁ', 'ᛃ', 'ᛇ', 'ᛈ', 'ᛉ', 'ᛋ', 'ᛏ', 'ᛒ', 'ᛖ', 'ᛗ',
            'ᛚ', 'ᛜ', 'ᛞ', 'ᛟ', 'ᛝ', 'ᛡ', 'ᛠ', '⸸', '☠', '♆',
            '☾', '☿', '♄', '♃', '♀', '♂', '⚶', '⚸'
        };

        public float CorruptionSpeed { get; set; } = 6f;

        public void ModifyVertices(TMP_TextInfo textInfo, int startIndex, int endIndex, float time)
        {
            if (textInfo == null || textInfo.characterCount == 0)
            {
                return;
            }

            TMP_Text textComponent = textInfo.textComponent;
            if (textComponent == null || string.IsNullOrEmpty(textComponent.text))
            {
                return;
            }

            float currentTime = Time.unscaledTime + time;
            int clampedStart = Mathf.Clamp(startIndex, 0, textInfo.characterCount - 1);
            int clampedEnd = Mathf.Clamp(endIndex, clampedStart, textInfo.characterCount - 1);

            char[] characters = textComponent.text.ToCharArray();
            bool changed = false;

            for (int i = clampedStart; i <= clampedEnd && i < characters.Length; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
                if (!characterInfo.isVisible || char.IsWhiteSpace(characters[i]))
                {
                    continue;
                }

                int glyphIndex = GetGlyphIndex(i, currentTime);
                char corrupted = CorruptionGlyphs[glyphIndex];

                if (characters[i] != corrupted)
                {
                    characters[i] = corrupted;
                    changed = true;
                }
            }

            if (changed)
            {
                textComponent.text = new string(characters);
            }
        }

        private int GetGlyphIndex(int index, float currentTime)
        {
            int tick = Mathf.FloorToInt(currentTime * CorruptionSpeed);
            int hash = (tick * 73856093) ^ (index * 19349663) ^ 0x5bd1e995;
            return Mathf.Abs(hash) % CorruptionGlyphs.Length;
        }
    }
}
