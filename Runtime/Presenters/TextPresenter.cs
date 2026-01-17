using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NovelUIKit.Runtime.Effects;
using TMPro;
using UnityEngine;

namespace NovelUIKit.Runtime.Presenters
{
    public sealed class TextPresenter : ITextPresenter
    {
        private readonly TMP_Text text;
        private bool isRunning;
        private bool skipRequested;

        public TextPresenter(TMP_Text text)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public bool IsRunning => isRunning;
        public bool IsSkipRequested => skipRequested;

        public async UniTask PresentAsync(string message, TextPresenterOptions options, CancellationToken cancellationToken)
        {
            if (isRunning)
            {
                throw new InvalidOperationException("TextPresenter is already running.");
            }

            options ??= TextPresenterOptions.Default;
            isRunning = true;
            skipRequested = false;

            try
            {
                text.richText = true;
                text.text = message ?? string.Empty;
                text.maxVisibleCharacters = 0;
                text.ForceMeshUpdate();

                var totalCharacters = text.textInfo.characterCount;
                for (var visibleCount = 0; visibleCount <= totalCharacters; visibleCount++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (skipRequested && options.RevealImmediatelyOnSkip)
                    {
                        text.maxVisibleCharacters = totalCharacters;
                        ApplyEffectRanges(options.EffectRanges, totalCharacters);
                        break;
                    }

                    text.maxVisibleCharacters = visibleCount;
                    ApplyEffectRanges(options.EffectRanges, visibleCount);

                    if (visibleCount < totalCharacters)
                    {
                        var delay = TimeSpan.FromSeconds(options.CharacterIntervalSeconds);
                        await UniTask.Delay(delay, options.UseUnscaledTime, cancellationToken: cancellationToken);
                    }
                }
            }
            finally
            {
                isRunning = false;
            }
        }

        public void RequestSkip()
        {
            skipRequested = true;
        }

        public void Reset()
        {
            skipRequested = false;
            isRunning = false;
            text.maxVisibleCharacters = 0;
        }

        private void ApplyEffectRanges(IReadOnlyList<TextEffectRange> ranges, int visibleCount)
        {
            if (ranges == null || ranges.Count == 0)
            {
                return;
            }

            var timeSeconds = Time.unscaledTime;

            foreach (var range in ranges)
            {
                if (range == null || range.Modifiers.Count == 0)
                {
                    continue;
                }

                var start = Math.Max(0, range.StartIndex);
                var end = Math.Min(range.StartIndex + range.Length - 1, visibleCount - 1);
                if (end < start)
                {
                    continue;
                }

                foreach (var modifier in range.Modifiers)
                {
                    modifier?.Apply(text, start, end, timeSeconds);
                }
            }
        }
    }
}
