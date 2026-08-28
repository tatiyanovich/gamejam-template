Shader "Environment/DesertFloor"
{
    Properties
    {
        [Header(Base)]
        _BaseColor ("Base Color", Color) = (0.85, 0.75, 0.55, 1)
        _MainTex ("Base Texture (optional)", 2D) = "white" {}
        _MainTex_Scale ("Base Texture Scale", Float) = 1

        [Header(Detail Map)]
        _DetailTex ("Detail / Height Map", 2D) = "gray" {}
        _DetailScale ("Detail Texture Scale", Float) = 1
        _DetailStrength ("Detail Strength", Range(0, 1)) = 0.3
        _DetailBlendMode ("Detail Blend: 0=Overlay 1=Multiply", Range(0, 1)) = 0

        [Header(Noise Anti Tiling)]
        _NoiseScale ("Noise Scale", Float) = 0.3
        _NoiseStrength ("Noise Strength (color variation)", Range(0, 0.5)) = 0.08
        _UVJitterStrength ("UV Jitter Strength", Range(0, 1)) = 0.015
        _ColorVariation ("Color Variation Tint", Color) = (0.9, 0.82, 0.6, 1)

        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "DesertFloor"
            Tags { "LightMode" = "Universal2D" }

            Cull [_Cull]
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 worldUV    : TEXCOORD1;
                float4 color      : COLOR;
            };

            TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);
            TEXTURE2D(_DetailTex);  SAMPLER(sampler_DetailTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _MainTex_ST;
                float  _MainTex_Scale;
                float4 _DetailTex_ST;
                float  _DetailScale;
                float  _DetailStrength;
                float  _DetailBlendMode;
                float  _NoiseScale;
                float  _NoiseStrength;
                float  _UVJitterStrength;
                float4 _ColorVariation;
            CBUFFER_END

            // --- Procedural noise (Value noise + FBM) ---

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f); // smoothstep

                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p, int octaves)
            {
                float value = 0.0;
                float amplitude = 0.5;
                for (int i = 0; i < octaves; i++)
                {
                    value += amplitude * valueNoise(p);
                    p *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            // --- Vertex ---

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldUV = worldPos.xy;

                OUT.color = IN.color;
                return OUT;
            }

            // --- Fragment ---

            half4 frag(Varyings IN) : SV_Target
            {
                float2 worldUV = IN.worldUV;

                // Noise-based UV jitter to break tiling
                float2 noiseUV = worldUV * _NoiseScale;
                float jitterX = fbm(noiseUV + float2(0, 17.3), 3);
                float jitterY = fbm(noiseUV + float2(31.7, 0), 3);
                float2 jitter = (float2(jitterX, jitterY) - 0.5) * _UVJitterStrength;

                // Sample base texture with jittered UVs (world-space tiling)
                float2 baseUV = worldUV * _MainTex_Scale + jitter;
                half4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, baseUV);
                half4 base = baseTex * _BaseColor;

                // Sample detail map at different scale (also world-space)
                float2 detailUV = worldUV * _DetailScale + jitter * 0.7;
                half4 detailTex = SAMPLE_TEXTURE2D(_DetailTex, sampler_DetailTex, detailUV);
                half detailGray = dot(detailTex.rgb, half3(0.299, 0.587, 0.114));

                // Blend detail: overlay or multiply
                half3 overlayBlend = base.rgb < 0.5
                    ? 2.0 * base.rgb * detailGray
                    : 1.0 - 2.0 * (1.0 - base.rgb) * (1.0 - detailGray);
                half3 multiplyBlend = base.rgb * lerp(half3(1,1,1), detailTex.rgb, _DetailStrength);
                half3 detailResult = lerp(
                    lerp(base.rgb, overlayBlend, _DetailStrength),
                    multiplyBlend,
                    _DetailBlendMode
                );

                // Color variation noise
                float colorNoise = fbm(worldUV * _NoiseScale * 1.7 + float2(77.7, 33.3), 4);
                half3 tintVariation = lerp(half3(1,1,1), _ColorVariation.rgb, colorNoise * _NoiseStrength * 4.0);
                half3 finalColor = detailResult * tintVariation;

                // Slight large-scale darkening for natural look
                float largeDarken = fbm(worldUV * _NoiseScale * 0.3 + float2(99.1, 11.4), 2);
                finalColor *= lerp(1.0, 0.92, largeDarken * _NoiseStrength * 3.0);

                return half4(finalColor, base.a) * IN.color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
