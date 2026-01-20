using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NovelUIKit.Effects;
using NovelUIKit.Effects.ScreenEffects;
using NovelUIKit.Effects.VertexModifiers;
using NovelUIKit.Runtime.Presenters;
using NovelUIKit.Runtime.TextPresenter;
using TMPro;
using UnityEngine;
using VContainer;
using ZLogger;

namespace NovelUIKit.Samples.BasicDemo
{
    public sealed class BasicDemoController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private TMP_Text demoText;
        [SerializeField] private ScreenNoiseEffect screenNoiseEffect;

        private readonly RubyTextHandler rubyTextHandler = new RubyTextHandler();
        private ITextPresenterFactory textPresenterFactory;
        private ITextPresenter textPresenter;
        private IGlitchEffectController glitchEffectController;

        [Inject]
        public void Construct(ITextPresenterFactory presenterFactory, IGlitchEffectController glitchEffectController)
        {
            textPresenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
            this.glitchEffectController = glitchEffectController ?? throw new ArgumentNullException(nameof(glitchEffectController));
        }

        private void Awake()
        {
            if (demoText == null)
            {
                ZLogger.LogWarning("BasicDemoController: demoText is not assigned.");
            }
        }

        private async UniTaskVoid Start()
        {
            if (demoText == null || textPresenterFactory == null)
            {
                ZLogger.LogWarning("BasicDemoController: Required references are missing. Demo will not start.");
                return;
            }

            textPresenter = textPresenterFactory.Create(demoText);

            ZLogger.LogInformation("Basic demo started.");
            await RunDemoAsync(this.GetCancellationTokenOnDestroy());
        }

        private async UniTask RunDemoAsync(CancellationToken ct)
        {
            await ShowTitleAsync(ct);
            await ShowNormalDemoAsync(ct);
            await ShowShakeDemoAsync(ct);
            await ShowGlitchDemoAsync(ct);
            await ShowScreenNoiseDemoAsync(ct);
            await ShowRubyDemoAsync(ct);
            await ShowCompletionAsync(ct);
        }

        private async UniTask ShowTitleAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Title");
            var options = TextPresenterOptions.Default;
            await PresentWithSkipAsync("NovelUIKit Basic Demo\nTap to start", options, ct);
            await WaitForTapAsync(ct);
        }

        private async UniTask ShowNormalDemoAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Normal typing");
            var options = TextPresenterOptions.Default;
            await PresentWithSkipAsync("通常の文字送りデモです。タップでスキップできます。", options, ct);
            await WaitForTapAsync(ct);
        }

        private async UniTask ShowShakeDemoAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Shake effect");
            var modifier = new ShakeModifier
            {
                Amplitude = 3.5f,
                Frequency = 18f
            };
            var options = new TextPresenterOptions
            {
                CharacterIntervalSeconds = 0.02f,
                EffectRanges = new[]
                {
                    new TextEffectRange(0, 999, new[] { modifier })
                }
            };

            await PresentWithSkipAsync("Shakeエフェクトのデモです。", options, ct);
            await WaitForTapAsync(ct);
        }

        private async UniTask ShowGlitchDemoAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Glitch effect");
            var modifier = new GlyphCorruptionModifier
            {
                CorruptionSpeed = 8f
            };
            var options = new TextPresenterOptions
            {
                CharacterIntervalSeconds = 0.02f,
                EffectRanges = new[]
                {
                    new TextEffectRange(0, 999, new[] { modifier })
                }
            };

            await glitchEffectController.ApplyGlyphCorruptionAsync(0, 10, 0.5f, ct);
            await PresentWithSkipAsync("Glitchエフェクトのデモです。", options, ct);
            await WaitForTapAsync(ct);
        }

        private async UniTask ShowScreenNoiseDemoAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Screen noise effect");
            await PresentWithSkipAsync("ScreenNoiseエフェクトのデモです。", TextPresenterOptions.Default, ct);

            if (screenNoiseEffect == null)
            {
                ZLogger.LogWarning("ScreenNoiseEffect is not assigned. Skipping noise playback.");
            }
            else
            {
                await screenNoiseEffect.PlayNoiseAsync(0.6f, 1.8f, ct);
            }

            await WaitForTapAsync(ct);
        }

        private async UniTask ShowRubyDemoAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Ruby text");
            const string rubySource = "<ruby>小説<rt>しょうせつ</rt></ruby>の<ruby>演出<rt>えんしゅつ</rt></ruby>を確認します。";
            var rubyDisplay = BuildRubyDisplayText(rubySource);
            await PresentWithSkipAsync(rubyDisplay, TextPresenterOptions.Default, ct);
            await WaitForTapAsync(ct);
        }

        private async UniTask ShowCompletionAsync(CancellationToken ct)
        {
            ZLogger.LogInformation("Demo step: Complete");
            await PresentWithSkipAsync("デモ完了です。お疲れさまでした。", TextPresenterOptions.Default, ct);
            await WaitForTapAsync(ct);
        }

        private async UniTask PresentWithSkipAsync(string message, TextPresenterOptions options, CancellationToken ct)
        {
            textPresenter.Reset();
            var presentTask = textPresenter.PresentAsync(message, options, ct);
            var tapTask = WaitForTapAsync(ct);

            var completed = await UniTask.WhenAny(presentTask, tapTask);
            if (completed == 1)
            {
                textPresenter.RequestSkip();
            }

            await presentTask;
        }

        private static UniTask WaitForTapAsync(CancellationToken ct)
        {
            return UniTask.WaitUntil(IsTapTriggered, cancellationToken: ct);
        }

        private static bool IsTapTriggered()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return true;
            }

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                return touch.phase == TouchPhase.Began;
            }

            return false;
        }

        private string BuildRubyDisplayText(string source)
        {
            var result = rubyTextHandler.Parse(source);
            if (result.Annotations.Count == 0)
            {
                return result.OutputText;
            }

            var builder = new StringBuilder(result.OutputText);
            var offset = 0;
            foreach (var annotation in result.Annotations)
            {
                var insertIndex = annotation.BaseStartIndex + annotation.BaseLength + offset;
                var insertText = $"（{annotation.RubyText}）";
                builder.Insert(insertIndex, insertText);
                offset += insertText.Length;
            }

            return builder.ToString();
        }
    }
}
