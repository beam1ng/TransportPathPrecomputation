Shader "Unlit/ImpulseResponse"
{
    SubShader
    {
        Blend One Zero
        Cull Off
        ZTest Always
        ZWrite Off

        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                uint vertexID : SV_VertexID;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertexCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _SharedBufferData;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                float2 uv = float2(v.instanceID / 100.0, v.vertexID);
                o.vertexCS = float4(uv * 2.0 - 1.0, 0, 1);
                return o;
            }

            half frag(v2f i) : SV_Target
            {
                return 1;
             }
            ENDHLSL
        }
    }
}