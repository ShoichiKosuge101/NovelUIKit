using System.Threading;
using Cysharp.Threading.Tasks;

namespace NovelUIKit.Effects
{
    public interface IGlitchEffectController
    {
        UniTask ApplyGlyphCorruptionAsync(int startIndex, int endIndex, float duration, CancellationToken ct = default);
        UniTask ApplyVertexDistortionAsync(int startIndex, int endIndex, DistortionType type, CancellationToken ct = default);
        UniTask PlayScreenNoiseAsync(float intensity, float duration, CancellationToken ct = default);
        void StopAllEffects();
    }

    public enum DistortionType
    {
        Crush,
        Melt,
        Shatter,
        Stretch
    }
}
