using TMPro;
using UnityEngine;

namespace NovelUIKit.Effects.VertexModifiers
{
    public sealed class ShakeModifier : IVertexModifier
    {
        public float Amplitude { get; set; } = 2f;
        public float Frequency { get; set; } = 20f;

        public void ModifyVertices(TMP_TextInfo textInfo, int startIndex, int endIndex, float time)
        {
            if (textInfo == null || textInfo.characterCount == 0)
            {
                return;
            }

            float currentTime = Time.unscaledTime + time;
            int clampedStart = Mathf.Clamp(startIndex, 0, textInfo.characterCount - 1);
            int clampedEnd = Mathf.Clamp(endIndex, clampedStart, textInfo.characterCount - 1);

            for (int i = clampedStart; i <= clampedEnd; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
                if (!characterInfo.isVisible)
                {
                    continue;
                }

                int meshIndex = characterInfo.materialReferenceIndex;
                int vertexIndex = characterInfo.vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

                float phase = currentTime * Frequency + i * 0.37f;
                Vector3 offset = new Vector3(
                    Mathf.Sin(phase) * Amplitude,
                    Mathf.Cos(phase * 1.3f) * Amplitude,
                    0f);

                vertices[vertexIndex] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }
        }
    }
}
