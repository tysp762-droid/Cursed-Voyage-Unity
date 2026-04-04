Shader "Environment Starter/Standard Intersection Mask URP"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        // Add other properties here...
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _Color;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_OUTPUT(Varyings, OUT);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float4 positionWS = mul(GetObjectToWorldMatrix(), float4(IN.positionOS, 1));
                OUT.positionWS = positionWS.xyz;
                OUT.positionCS = mul(GetWorldToHClipMatrix(), positionWS);
                OUT.normalWS = normalize(mul((float3x3)GetObjectToWorldMatrix(), IN.normalOS));
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample texture
                half4 albedo = tex2D(_MainTex, IN.uv) * _Color;

                // Simple lighting example (replace with PBR lighting)
                half3 normal = normalize(IN.normalWS);
                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half NdotL = max(dot(normal, lightDir), 0);

                half3 color = albedo.rgb * NdotL;

                return half4(color, albedo.a);
            }

            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
