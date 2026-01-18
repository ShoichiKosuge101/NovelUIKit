using System;
using System.Collections.Generic;
using NovelUIKit.Runtime.Effects;

namespace NovelUIKit.Runtime.Presenters
{
    [Serializable]
    public sealed class TextPresenterOptions
    {
        public static TextPresenterOptions Default => new();

        public float CharacterIntervalSeconds { get; set; } = 0.03f;
        public bool UseUnscaledTime { get; set; } = true;
        public bool RevealImmediatelyOnSkip { get; set; } = true;
        public IReadOnlyList<TextEffectRange> EffectRanges { get; set; } = Array.Empty<TextEffectRange>();
    }

    [Serializable]
    public sealed class TextEffectRange
    {
        public int StartIndex { get; }
        public int Length { get; }
        public IReadOnlyList<IVertexModifier> Modifiers { get; }

        public TextEffectRange(int startIndex, int length, IReadOnlyList<IVertexModifier> modifiers)
        {
            StartIndex = startIndex;
            Length = length;
            Modifiers = modifiers ?? Array.Empty<IVertexModifier>();
        }
    }
}
