Shader "UI/ScreenSpaceBlik"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _BlikColor ("Blik Color", Color) = (1,1,1,1)
        _Interval  ("Interval", Float) = 1
        _Speed     ("Speed", Float) = 1
        _Size      ("Size", Float) = 1
        _Rotation  ("Rotation", Range(-1, 1)) = 0.5

        [Toggle(BLIK_ENABLED)] _isEnabled ("Enable Blik", Float) = 1

        _StencilComp      ("Stencil Comparison", Float) = 8
        _Stencil          ("Stencil ID",         Float) = 0
        _StencilOp        ("Stencil Operation",  Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask",  Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Stencil
        {
            Ref       [_Stencil]
            Comp      [_StencilComp]
            Pass      [_StencilOp]
            ReadMask  [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   2.0

            #pragma shader_feature_local BLIK_ENABLED
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define PI 3.14159265359

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half4  color       : COLOR;
                float2 uv          : TEXCOORD0;
                float4 worldPos    : TEXCOORD1;
                float2 screenPos   : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4  _Color;
                half4  _BlikColor;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float  _Rotation;
                float  _Interval;
                float  _Speed;
                float  _Size;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            float UIClipRect(float2 worldPos, float4 clipRect)
            {
                float2 inside = step(clipRect.xy, worldPos) * step(worldPos, clipRect.zw);
                return inside.x * inside.y;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                OUT.color       = IN.color * _Color;
                OUT.worldPos    = IN.positionOS;
                OUT.screenPos   = OUT.positionHCS.xy / OUT.positionHCS.w;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                #ifdef BLIK_ENABLED
                    float aspect = _ScreenParams.x / _ScreenParams.y;

                    // Diagonal sweep coordinate combining x and y based on _Rotation.
                    float p = IN.screenPos.x * _Rotation
                            + (IN.screenPos.y / aspect) * (1.0 - _Rotation) * _ProjectionParams.x;

                    float t = _Time.y * _Speed;
                    float k = sin((p + t) * PI / _Interval);

                    float size = max(_Size, 1e-4);
                    float blik = 1.0 - pow(k * _Interval / size, 4.0);
                    blik = saturate(blik * 1000.0);                // sharpen to a thin band
                    blik *= _BlikColor.a;

                    col.rgb = lerp(col.rgb, _BlikColor.rgb, blik);
                #endif

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UIClipRect(IN.worldPos.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}
