using TMPro;

namespace NovelUIKit.Effects.VertexModifiers
{
    public interface IVertexModifier
    {
        void ModifyVertices(TMP_TextInfo textInfo, int startIndex, int endIndex, float time);
    }
}
