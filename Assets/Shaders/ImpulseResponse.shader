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

            // struct _PathData
            // {
            //     float timeDelay;
            //     float contributions[12];
            //     float3 inputPosition;
            //     float3 outputPosition;
            // };

            float3 _SourcePositionWS;

            float3 _ListenerPositionWS;
            int _TimeSamples;
            float _TimeRange;
            int _SampleFrequenciesCount;

            v2f vert(appdata v)
            {
                v2f o;

                float2 uv = float2(
                    (v.instanceID + 0.5) / (float)_TimeSamples,
                    v.vertexID);
                    // (0.5 + v.vertexID * ((float)_SampleFrequenciesCount - 1.0 /(float)_SampleFrequenciesCount))/(float)_SampleFrequenciesCount);

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