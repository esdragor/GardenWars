Shader "Unlit/TextureMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pattern ("Pattern", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _Pattern;

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.worldPos = mul(UNITY_MATRIX_M, float4( v.vertex.xyz, 1 ) );
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv;
           
                return o;
            }

           

            float4 frag(Interpolators i) : SV_Target {

                float2 topDownProjection = i.worldPos.xz;
                float4 col = tex2D(_MainTex, topDownProjection );
                float pattern = tex2D( _Pattern, i.uv ).x;

                float4 finalColor = lerp( float4(1,0,0,1), col, pattern );

                return finalColor;
            }
            ENDCG
        }
    }
}
