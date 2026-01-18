using TMPro;
using UnityEngine;

namespace NovelUIKit.Effects.VertexModifiers
{
    public enum DistortionMode
    {
        Crush,
        Melt,
        Shatter,
        Stretch
    }

    public sealed class DistortionModifier : IVertexModifier
    {
        public DistortionMode Mode { get; set; } = DistortionMode.Crush;
        public float Amplitude { get; set; } = 4f;
        public float Frequency { get; set; } = 8f;

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

                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;
                float phase = currentTime * Frequency + i * 0.23f;

                switch (Mode)
                {
                    case DistortionMode.Crush:
                        ApplyCrush(vertices, vertexIndex, center, phase);
                        break;
                    case DistortionMode.Melt:
                        ApplyMelt(vertices, vertexIndex, center, phase);
                        break;
                    case DistortionMode.Shatter:
                        ApplyShatter(vertices, vertexIndex, center, phase, i);
                        break;
                    case DistortionMode.Stretch:
                        ApplyStretch(vertices, vertexIndex, center, phase);
                        break;
                }
            }
        }

        private void ApplyCrush(Vector3[] vertices, int vertexIndex, Vector3 center, float phase)
        {
            float crush = Mathf.Lerp(1f, 0.4f, (Mathf.Sin(phase) + 1f) * 0.5f);
            for (int i = 0; i < 4; i++)
            {
                Vector3 offset = vertices[vertexIndex + i] - center;
                vertices[vertexIndex + i] = center + offset * crush;
            }
        }

        private void ApplyMelt(Vector3[] vertices, int vertexIndex, Vector3 center, float phase)
        {
            float melt = Mathf.Lerp(0.2f, 1f, (Mathf.Sin(phase) + 1f) * 0.5f);
            for (int i = 0; i < 4; i++)
            {
                float depth = Mathf.InverseLerp(center.y - Amplitude, center.y + Amplitude, vertices[vertexIndex + i].y);
                Vector3 offset = new Vector3(0f, -Amplitude * melt * depth, 0f);
                vertices[vertexIndex + i] += offset;
            }
        }

        private void ApplyShatter(Vector3[] vertices, int vertexIndex, Vector3 center, float phase, int index)
        {
            float noise = Mathf.Sin(phase + index * 1.7f);
            Vector3 jitter = new Vector3(Mathf.Cos(phase * 1.3f + index), noise, 0f) * Amplitude;
            for (int i = 0; i < 4; i++)
            {
                vertices[vertexIndex + i] += jitter * (0.5f + i * 0.1f);
            }
        }

        private void ApplyStretch(Vector3[] vertices, int vertexIndex, Vector3 center, float phase)
        {
            float stretchX = Mathf.Lerp(0.6f, 1.4f, (Mathf.Sin(phase) + 1f) * 0.5f);
            float stretchY = Mathf.Lerp(1.4f, 0.6f, (Mathf.Cos(phase) + 1f) * 0.5f);
            Vector3 scale = new Vector3(stretchX, stretchY, 1f);

            for (int i = 0; i < 4; i++)
            {
                Vector3 offset = vertices[vertexIndex + i] - center;
                vertices[vertexIndex + i] = center + Vector3.Scale(offset, scale);
            }
        }
    }
}
