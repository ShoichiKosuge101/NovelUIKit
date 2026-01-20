using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using ZLogger;

namespace NovelUIKit.Effects
{
    public sealed class GlitchEffectController : IGlitchEffectController
    {
        private static readonly ILogger Logger = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            builder.AddZLoggerConsole();
        }).CreateLogger<GlitchEffectController>();

        private CancellationTokenSource _stopCts = new CancellationTokenSource();
        private Subject<Unit> _stopSignal = new Subject<Unit>();

        public async UniTask ApplyGlyphCorruptionAsync(int startIndex, int endIndex, float duration, CancellationToken ct = default)
        {
            ValidateRange(startIndex, endIndex);
            var clampedDuration = Mathf.Max(0f, duration);

            Logger.LogInformation(
                "Applying glyph corruption from {StartIndex} to {EndIndex} for {Duration}s.",
                startIndex,
                endIndex,
                clampedDuration);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _stopCts.Token);
            var effectTask = UniTask.Delay(TimeSpan.FromSeconds(clampedDuration), cancellationToken: linkedCts.Token);
            var stopTask = _stopSignal.First().ToUniTask(cancellationToken: linkedCts.Token);

            var completed = await UniTask.WhenAny(effectTask, stopTask);
            linkedCts.Cancel();
            if (completed == 1)
            {
                return;
            }

            await effectTask;
        }

        public async UniTask ApplyVertexDistortionAsync(int startIndex, int endIndex, DistortionType type, CancellationToken ct = default)
        {
            ValidateRange(startIndex, endIndex);

            Logger.LogInformation(
                "Applying vertex distortion {DistortionType} from {StartIndex} to {EndIndex}.",
                type,
                startIndex,
                endIndex);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _stopCts.Token);
            var effectTask = UniTask.DelayFrame(1, cancellationToken: linkedCts.Token);
            var stopTask = _stopSignal.First().ToUniTask(cancellationToken: linkedCts.Token);

            var completed = await UniTask.WhenAny(effectTask, stopTask);
            linkedCts.Cancel();
            if (completed == 1)
            {
                return;
            }

            await effectTask;
        }

        public async UniTask PlayScreenNoiseAsync(float intensity, float duration, CancellationToken ct = default)
        {
            var clampedIntensity = Mathf.Clamp01(intensity);
            var clampedDuration = Mathf.Max(0f, duration);

            Logger.LogInformation(
                "Playing screen noise with intensity {Intensity} for {Duration}s.",
                clampedIntensity,
                clampedDuration);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _stopCts.Token);
            var effectTask = UniTask.Delay(TimeSpan.FromSeconds(clampedDuration), cancellationToken: linkedCts.Token);
            var stopTask = _stopSignal.First().ToUniTask(cancellationToken: linkedCts.Token);

            var completed = await UniTask.WhenAny(effectTask, stopTask);
            linkedCts.Cancel();
            if (completed == 1)
            {
                return;
            }

            await effectTask;
        }

        public void StopAllEffects()
        {
            Logger.LogInformation("Stopping all glitch effects.");

            _stopSignal.OnNext(Unit.Default);
            _stopSignal.OnCompleted();
            _stopSignal.Dispose();

            _stopCts.Cancel();
            _stopCts.Dispose();

            _stopCts = new CancellationTokenSource();
            _stopSignal = new Subject<Unit>();
        }

        private static void ValidateRange(int startIndex, int endIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex must be >= 0.");
            }

            if (endIndex < startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex), "endIndex must be >= startIndex.");
            }
        }
    }
}
