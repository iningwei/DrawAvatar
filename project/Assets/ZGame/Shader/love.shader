// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/love" {  
    Properties {   
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        _Size ("爱心大小", Range(0, 10)) = 1  
  
    }   
       
    SubShader {      
        Tags {"Queue" = "Transparent"}        
        ZWrite Off  
  
        Pass {       
            CGPROGRAM       
            #pragma vertex vert  
            #pragma fragment frag       
            #include "UnityCG.cginc"  
            #pragma target 3.0     
         
            sampler2D _MainTex;   
            float _Size;  
           
            struct v2f {       
                float4 pos:SV_POSITION;       
                float4 srcPos : TEXCOORD0;    
            };     
     
            v2f vert(appdata_base v) {     
                v2f o;     
                o.pos = UnityObjectToClipPos (v.vertex);     
                // 根据当前顶点计算在屏幕上的位置  
                o.srcPos = ComputeScreenPos(o.pos);    
                return o;  
            }     
     
            fixed4 frag(v2f i) : COLOR0 {  
                // 当前顶点在屏幕上的位置,倒置方向
                float2 center = -1*(i.srcPos.xy/i.srcPos.w);    
                float3 col = float3(0,0,0);  
          
                for(int j = 0; j < 30; j ++)  
                {  
                    // 求每个点的角度【将圆形变成30边形，计算每个顶点的角度】  
                    float an = 6.2831 * float(j) / 30;  
                    // 求爱心  
                    float2 pointPos = float2(  
                    16 * pow(sin(an), 3),  
                    13 * cos(an) - 5 * cos(2 * an) - 2 * cos(3 * an) - cos(4 * an)  
                     );  
                    // 在UV值域范围上半径大小为【size/_ScreenParams.xy】  
                    // 所以新的UV偏移量为【center + pointPos * 单位半径】  
                    col = max(col, tex2D(_MainTex, center + pointPos * _Size / _ScreenParams.xy).xyz);  
                }  
                return fixed4(col, 1);   
            }    
            ENDCG    
        }//end pass  
    }// end subshader  
    FallBack Off     
} 
