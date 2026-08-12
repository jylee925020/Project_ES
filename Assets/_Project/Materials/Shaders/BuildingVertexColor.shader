Shader "Custom/BuildingVertexColor"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogAmount ("Fog Amount", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _FogColor;
                half _FogAmount;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.color = input.color;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                return lerp(input.color, _FogColor, _FogAmount);
            }

            ENDHLSL
        }
    }
}