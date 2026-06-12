Shader "My/PostProcess/SceneScanner"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _ScanDistance("Scan Distance",float)=0
        _ScanWidth("Scan Width",float)=0.2
        _ScanColor("Scan Color",Color)=(0,1,0,0)
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;               
                float4 vertex : SV_POSITION;
            };

            

            sampler2D _MainTex;
            float4 _MainTex_ST;

             sampler2D _CameraDepthTexture;

             float _ScanDistance;
			float _ScanWidth;
			float4 _ScanColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
                fixed4 col = tex2D(_MainTex, i.uv);

                //获取深度信息
               // float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));   
               //or
               float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				depth = Linear01Depth(depth);
                 if (depth < _ScanDistance && depth > _ScanDistance - _ScanWidth && depth < 1)
				{
					float diff = 1 - (_ScanDistance - depth) / (_ScanWidth);
					_ScanColor *= diff;
					return col + _ScanColor;
				}

                return col;
            }
            ENDCG
        }
    }
}
