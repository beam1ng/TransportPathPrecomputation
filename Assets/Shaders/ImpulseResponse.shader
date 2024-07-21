Shader "Unlit/ImpulseResponse"
{
    SubShader
    {
        Blend One One
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
                uint instanceID : TEXCOORD0;
                float timeDelay : TEXCOORD1;
                float4 vertexCS : SV_POSITION;
            };

            struct PathData
            {
                float timeDelay;
                float contributions[12];
                float3 inputPosition;
                float3 inputNormal;
                float3 inputPathDir;
                float3 outputPosition;
                float3 outputNormal;
                float3 outputPathDir;
            };

            float BRDF_Specular(float3 inDir, float3 norm, float3 outDir)
            {
                return dot(reflect(inDir,norm),outDir);
            }
            
            CBUFFER_START(UnityPerMaterial)
                float4 _ArgsBuffer;
            CBUFFER_END

            StructuredBuffer<PathData> _SurfacePaths;
            
            float3 _SourcePositionWS;
            float3 _ListenerPositionWS;
            int _TimeSamples;
            float _TimeRange;
            int _SampleFrequenciesCount;

            v2f vert(appdata v)
            {
                v2f o;

                PathData pathData = _SurfacePaths[v.instanceID];

                float2 uv = float2(
                    pathData.timeDelay / 0.5,
                    v.vertexID);
                
                o.vertexCS = float4(uv * 2.0 - 1.0, 0, 1);
                o.instanceID = v.instanceID;
                return o;
            }

            half frag(v2f i) : SV_Target
            {
                PathData pathData = _SurfacePaths[i.instanceID];
                float contribution = pathData.contributions[(i.vertexCS.y-0.499)/_SampleFrequenciesCount];
                
                contribution *= BRDF_Specular(
                    normalize(_SourcePositionWS - pathData.inputPosition),
                    pathData.inputNormal,
                    pathData.inputPathDir);
                
                contribution *= BRDF_Specular(
                    normalize(_ListenerPositionWS - pathData.outputPosition),
                    pathData.outputNormal,
                    pathData.outputPathDir);

                float totalPathLength = pathData.timeDelay * 343.0;
                totalPathLength += distance(_SourcePositionWS,pathData.inputPosition);
                totalPathLength += distance(_ListenerPositionWS,pathData.outputPosition);

                contribution /=(totalPathLength * totalPathLength);
                
                return  contribution * 100.0;
            }
            ENDHLSL
        }
    }
}