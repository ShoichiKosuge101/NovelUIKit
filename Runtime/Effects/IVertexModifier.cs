using TMPro;

namespace NovelUIKit.Runtime.Effects
{
    public interface IVertexModifier
    {
        void Apply(TMP_Text text, int startIndex, int endIndex, float timeSeconds);
    }
}
