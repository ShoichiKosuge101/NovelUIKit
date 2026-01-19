using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.Rendering;
using ZLogger;

namespace NovelUIKit.Effects.ScreenEffects
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class ScreenNoiseEffect : MonoBehaviour
    {
        private static readonly ILogger Logger = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            builder.AddZLoggerConsole();
        }).CreateLogger<ScreenNoiseEffect>();

        [Header("Shader")]
        [SerializeField] private Shader screenNoiseShader;

        [Header("Noise Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float baseNoiseIntensity = 0.15f;
        [Range(0f, 1f)]
        [SerializeField] private float scanlineIntensity = 0.35f;
        [Range(0f, 1f)]
        [SerializeField] private float chromaticAberrationIntensity = 0.2f;
        [SerializeField] private Vector2 noiseSpeed = new Vector2(1.1f, 1.9f);

        private static readonly int NoiseIntensityId = Shader.PropertyToID("_NoiseIntensity");
        private static readonly int ScanlineIntensityId = Shader.PropertyToID("_ScanlineIntensity");
        private static readonly int ChromaticAberrationId = Shader.PropertyToID("_ChromaticAberration");
        private static readonly int NoiseSpeedId = Shader.PropertyToID("_NoiseSpeed");

        private Camera targetCamera;
        private Material materialInstance;
        private CancellationTokenSource noiseCts;
        private bool isPipelineSubscribed;
        private float runtimeNoiseIntensity;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            EnsureMaterial();
            if (GraphicsSettings.currentRenderPipeline != null)
            {
                RenderPipelineManager.beginCameraRendering += HandleBeginCameraRendering;
                isPipelineSubscribed = true;
            }
        }

        private void OnDisable()
        {
            if (isPipelineSubscribed)
            {
                RenderPipelineManager.beginCameraRendering -= HandleBeginCameraRendering;
                isPipelineSubscribed = false;
            }

            CancelNoise();
        }

        private void OnDestroy()
        {
            if (materialInstance != null)
            {
                Destroy(materialInstance);
                materialInstance = null;
            }

            CancelNoise();
        }

        public void SetBaseNoise(float intensity)
        {
            baseNoiseIntensity = Mathf.Clamp01(intensity);
        }

        public void SetScanline(float intensity)
        {
            scanlineIntensity = Mathf.Clamp01(intensity);
        }

        public void SetChromaticAberration(float intensity)
        {
            chromaticAberrationIntensity = Mathf.Clamp01(intensity);
        }

        public void SetNoiseSpeed(Vector2 speed)
        {
            noiseSpeed = speed;
        }

        public UniTask PlayNoiseAsync(float intensity, float duration, CancellationToken cancellationToken = default)
        {
            if (duration <= 0f)
            {
                runtimeNoiseIntensity = Mathf.Clamp01(intensity);
                ApplyMaterialParameters();
                return UniTask.CompletedTask;
            }

            CancelNoise();
            noiseCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            runtimeNoiseIntensity = Mathf.Clamp01(intensity);

            Logger.ZLogInformation($"ScreenNoiseEffect start. intensity={runtimeNoiseIntensity} duration={duration}");

            return RunNoiseAsync(duration, noiseCts.Token);
        }

        public void StopNoise()
        {
            CancelNoise();
            runtimeNoiseIntensity = 0f;
            ApplyMaterialParameters();
            Logger.ZLogInformation("ScreenNoiseEffect stopped.");
        }

        private async UniTask RunNoiseAsync(float duration, CancellationToken cancellationToken)
        {
            try
            {
                ApplyMaterialParameters();
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    runtimeNoiseIntensity = 0f;
                    ApplyMaterialParameters();
                    Logger.ZLogInformation("ScreenNoiseEffect finished.");
                }
            }
        }

        private void EnsureMaterial()
        {
            if (materialInstance != null)
            {
                return;
            }

            if (screenNoiseShader == null)
            {
                screenNoiseShader = Shader.Find("NovelUIKit/ScreenNoise");
            }

            if (screenNoiseShader == null)
            {
                Logger.ZLogWarning("ScreenNoise shader not found. Assign it to ScreenNoiseEffect.");
                return;
            }

            materialInstance = new Material(screenNoiseShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        private void CancelNoise()
        {
            if (noiseCts == null)
            {
                return;
            }

            noiseCts.Cancel();
            noiseCts.Dispose();
            noiseCts = null;
        }

        private void ApplyMaterialParameters()
        {
            if (materialInstance == null)
            {
                return;
            }

            materialInstance.SetFloat(NoiseIntensityId, Mathf.Clamp01(baseNoiseIntensity + runtimeNoiseIntensity));
            materialInstance.SetFloat(ScanlineIntensityId, Mathf.Clamp01(scanlineIntensity));
            materialInstance.SetFloat(ChromaticAberrationId, Mathf.Clamp01(chromaticAberrationIntensity));
            materialInstance.SetVector(NoiseSpeedId, noiseSpeed);
        }

        private void HandleBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (materialInstance == null || camera != targetCamera)
            {
                return;
            }

            if (runtimeNoiseIntensity <= 0f && baseNoiseIntensity <= 0f && scanlineIntensity <= 0f)
            {
                return;
            }

            ApplyMaterialParameters();

            var commandBuffer = CommandBufferPool.Get("ScreenNoiseEffect");
            commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, materialInstance);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (materialInstance == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            if (runtimeNoiseIntensity <= 0f && baseNoiseIntensity <= 0f && scanlineIntensity <= 0f)
            {
                Graphics.Blit(source, destination);
                return;
            }

            ApplyMaterialParameters();
            Graphics.Blit(source, destination, materialInstance);
        }
    }
}
