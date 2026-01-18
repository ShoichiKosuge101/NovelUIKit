Shader "NovelUIKit/ScreenNoise"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.15
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.35
        _ChromaticAberration ("Chromatic Aberration", Range(0, 1)) = 0.2
        _NoiseSpeed ("Noise Speed", Vector) = (1.1, 1.9, 0, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "URP"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _NoiseIntensity;
                float _ScanlineIntensity;
                float _ChromaticAberration;
                float2 _NoiseSpeed;
            CBUFFER_END

            float Hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 78.233);
                return frac(p.x * p.y);
            }

            float Noise(float2 uv)
            {
                float2 grid = floor(uv * 240.0);
                float2 f = frac(uv * 240.0);
                float a = Hash(grid + float2(0.0, 0.0));
                float b = Hash(grid + float2(1.0, 0.0));
                float c = Hash(grid + float2(0.0, 1.0));
                float d = Hash(grid + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float time = _Time.y;

                float2 drift = uv + _NoiseSpeed * time;
                float noiseValue = Noise(drift);
                float scanline = sin((uv.y + time * _NoiseSpeed.y) * 400.0) * 0.5 + 0.5;

                float chromaOffset = _ChromaticAberration * 0.003;
                float2 offset = float2(chromaOffset, 0.0);

                float3 baseColor;
                baseColor.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset).r;
                baseColor.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).g;
                baseColor.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - offset).b;

                float noiseMix = lerp(1.0, noiseValue, _NoiseIntensity);
                float scanMix = lerp(1.0, scanline, _ScanlineIntensity);

                float3 finalColor = baseColor * noiseMix * scanMix;
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "BuiltIn"

            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _NoiseIntensity;
            float _ScanlineIntensity;
            float _ChromaticAberration;
            float2 _NoiseSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float Hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 78.233);
                return frac(p.x * p.y);
            }

            float Noise(float2 uv)
            {
                float2 grid = floor(uv * 240.0);
                float2 f = frac(uv * 240.0);
                float a = Hash(grid + float2(0.0, 0.0));
                float b = Hash(grid + float2(1.0, 0.0));
                float c = Hash(grid + float2(0.0, 1.0));
                float d = Hash(grid + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            v2f Vert(appdata input)
            {
                v2f output;
                output.positionCS = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            fixed4 Frag(v2f input) : SV_Target
            {
                float2 uv = input.uv;
                float time = _Time.y;

                float2 drift = uv + _NoiseSpeed * time;
                float noiseValue = Noise(drift);
                float scanline = sin((uv.y + time * _NoiseSpeed.y) * 400.0) * 0.5 + 0.5;

                float chromaOffset = _ChromaticAberration * 0.003;
                float2 offset = float2(chromaOffset, 0.0);

                float3 baseColor;
                baseColor.r = tex2D(_MainTex, uv + offset).r;
                baseColor.g = tex2D(_MainTex, uv).g;
                baseColor.b = tex2D(_MainTex, uv - offset).b;

                float noiseMix = lerp(1.0, noiseValue, _NoiseIntensity);
                float scanMix = lerp(1.0, scanline, _ScanlineIntensity);

                float3 finalColor = baseColor * noiseMix * scanMix;
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
