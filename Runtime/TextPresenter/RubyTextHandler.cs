using System;
using System.Collections.Generic;

namespace NovelUIKit.Runtime.TextPresenter
{
    public sealed class RubyTextHandler
    {
        public RubyTextResult Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var output = new System.Text.StringBuilder(text.Length);
            var annotations = new List<RubyAnnotation>();
            var visibleIndex = 0;
            var index = 0;

            while (index < text.Length)
            {
                if (TryConsumeTag(text, index, "ruby", out var tagEnd))
                {
                    var rubyEnd = text.IndexOf("</ruby>", tagEnd + 1, StringComparison.Ordinal);
                    if (rubyEnd == -1)
                    {
                        output.Append(text[index]);
                        visibleIndex++;
                        index++;
                        continue;
                    }

                    var rubyContent = text.Substring(tagEnd + 1, rubyEnd - tagEnd - 1);
                    if (!TryExtractRuby(rubyContent, out var baseText, out var rubyText))
                    {
                        AppendWithVisibleCount(output, rubyContent, ref visibleIndex);
                        index = rubyEnd + "</ruby>".Length;
                        continue;
                    }

                    var baseStart = visibleIndex;
                    AppendWithVisibleCount(output, baseText, ref visibleIndex);
                    var baseLength = visibleIndex - baseStart;

                    if (baseLength > 0)
                    {
                        annotations.Add(new RubyAnnotation(baseStart, baseLength, rubyText));
                    }

                    index = rubyEnd + "</ruby>".Length;
                    continue;
                }

                if (text[index] == '<')
                {
                    if (TryConsumeAnyTag(text, index, out var endIndex))
                    {
                        output.Append(text, index, endIndex - index + 1);
                        index = endIndex + 1;
                        continue;
                    }
                }

                output.Append(text[index]);
                visibleIndex++;
                index++;
            }

            return new RubyTextResult(output.ToString(), annotations);
        }

        public int GetVisibleIndexAtRawPosition(string text, int rawIndex)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (rawIndex < 0 || rawIndex > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(rawIndex));
            }

            var visibleIndex = 0;
            var index = 0;
            var inRt = false;

            while (index < rawIndex)
            {
                if (text[index] == '<' && TryConsumeAnyTag(text, index, out var endIndex))
                {
                    var tagName = GetTagName(text, index + 1, endIndex - 1);
                    if (tagName == "rt")
                    {
                        inRt = true;
                    }
                    else if (tagName == "/rt")
                    {
                        inRt = false;
                    }

                    index = endIndex + 1;
                    continue;
                }

                if (!inRt)
                {
                    visibleIndex++;
                }

                index++;
            }

            return visibleIndex;
        }

        private static void AppendWithVisibleCount(System.Text.StringBuilder output, string text, ref int visibleIndex)
        {
            var index = 0;
            while (index < text.Length)
            {
                if (text[index] == '<' && TryConsumeAnyTag(text, index, out var endIndex))
                {
                    output.Append(text, index, endIndex - index + 1);
                    index = endIndex + 1;
                    continue;
                }

                output.Append(text[index]);
                visibleIndex++;
                index++;
            }
        }

        private static bool TryExtractRuby(string content, out string baseText, out string rubyText)
        {
            var rtStart = FindTagStart(content, "rt");
            if (rtStart == -1)
            {
                baseText = string.Empty;
                rubyText = string.Empty;
                return false;
            }

            var rtTagEnd = content.IndexOf('>', rtStart);
            if (rtTagEnd == -1)
            {
                baseText = string.Empty;
                rubyText = string.Empty;
                return false;
            }

            var rtEnd = content.IndexOf("</rt>", rtTagEnd + 1, StringComparison.Ordinal);
            if (rtEnd == -1 || rtEnd <= rtTagEnd)
            {
                baseText = string.Empty;
                rubyText = string.Empty;
                return false;
            }

            baseText = content.Substring(0, rtStart);
            rubyText = content.Substring(rtTagEnd + 1, rtEnd - rtTagEnd - 1);
            return true;
        }

        private static bool TryConsumeTag(string text, int startIndex, string tagName, out int endIndex)
        {
            endIndex = -1;
            if (startIndex + tagName.Length + 2 > text.Length)
            {
                return false;
            }

            if (text[startIndex] != '<')
            {
                return false;
            }

            var nameStart = startIndex + 1;
            if (!text.AsSpan(nameStart).StartsWith(tagName.AsSpan(), StringComparison.Ordinal))
            {
                return false;
            }

            var nameEnd = nameStart + tagName.Length;
            if (nameEnd >= text.Length)
            {
                return false;
            }

            var nextChar = text[nameEnd];
            if (nextChar != '>' && !char.IsWhiteSpace(nextChar))
            {
                return false;
            }

            var closeIndex = text.IndexOf('>', nameEnd);
            if (closeIndex == -1)
            {
                return false;
            }

            endIndex = closeIndex;
            return true;
        }

        private static bool TryConsumeAnyTag(string text, int startIndex, out int endIndex)
        {
            endIndex = -1;
            if (text[startIndex] != '<')
            {
                return false;
            }

            var closeIndex = text.IndexOf('>', startIndex + 1);
            if (closeIndex == -1)
            {
                return false;
            }

            endIndex = closeIndex;
            return true;
        }

        private static string GetTagName(string text, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex + 1;
            var tag = text.Substring(startIndex, length);
            var spaceIndex = tag.IndexOf(' ');
            return spaceIndex == -1 ? tag : tag.Substring(0, spaceIndex);
        }

        private static int FindTagStart(string text, string tagName)
        {
            var searchStart = 0;
            while (searchStart < text.Length)
            {
                var startIndex = text.IndexOf('<', searchStart);
                if (startIndex == -1)
                {
                    return -1;
                }

                var nameStart = startIndex + 1;
                if (nameStart + tagName.Length <= text.Length &&
                    text.AsSpan(nameStart).StartsWith(tagName.AsSpan(), StringComparison.Ordinal))
                {
                    var nameEnd = nameStart + tagName.Length;
                    if (nameEnd < text.Length)
                    {
                        var nextChar = text[nameEnd];
                        if (nextChar == '>' || char.IsWhiteSpace(nextChar))
                        {
                            return startIndex;
                        }
                    }
                }

                searchStart = startIndex + 1;
            }

            return -1;
        }
    }

    public sealed class RubyTextResult
    {
        public RubyTextResult(string outputText, IReadOnlyList<RubyAnnotation> annotations)
        {
            OutputText = outputText ?? throw new ArgumentNullException(nameof(outputText));
            Annotations = annotations ?? throw new ArgumentNullException(nameof(annotations));
        }

        public string OutputText { get; }

        public IReadOnlyList<RubyAnnotation> Annotations { get; }
    }

    public sealed class RubyAnnotation
    {
        public RubyAnnotation(int baseStartIndex, int baseLength, string rubyText)
        {
            BaseStartIndex = baseStartIndex;
            BaseLength = baseLength;
            RubyText = rubyText ?? throw new ArgumentNullException(nameof(rubyText));
        }

        public int BaseStartIndex { get; }

        public int BaseLength { get; }

        public string RubyText { get; }
    }
}
