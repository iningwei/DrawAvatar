//https://en.wikibooks.org/wiki/Cg_Programming/Unity/Billboards
//not work at all. do not know the reason
Shader "My/Billboard/Cg  shader for billboards" {
   Properties {
      _MainTex ("Texture Image", 2D) = "white" {}
      _ScaleX ("Scale X", Float) = 1.0
      _ScaleY ("Scale Y", Float) = 1.0
   }
   SubShader {
       
       Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
       Cull Off
       ZWrite Off
       Blend SrcAlpha OneMinusSrcAlpha
      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag
         #include "UnityCG.cginc"
         
           sampler2D _MainTex;   
         float4 _MainTex_ST;
           float _ScaleX;
           float _ScaleY;

         struct vertexInput {
            float4 vertex : POSITION;
            float2 tex : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float2 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;

            output.pos = mul(UNITY_MATRIX_P, 
              UnityObjectToViewPos( float4(0.0, 0.0, 0.0, 1.0))
              + float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
              * float4(_ScaleX, _ScaleY, 1.0, 1.0));
 

             output.tex  = TRANSFORM_TEX(input.tex, _MainTex);
      

            return output;
         }
 
         float4 frag(vertexOutput input) : SV_Target
         {
              fixed4 col = tex2D(_MainTex, input.tex );   
                
               return col;
         }
 
         ENDCG
      }
   }
}



