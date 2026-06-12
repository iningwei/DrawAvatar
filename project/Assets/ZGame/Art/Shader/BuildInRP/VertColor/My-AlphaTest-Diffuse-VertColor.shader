//https://blog.csdn.net/weixin_30616969/article/details/95424748
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "My/VertColor/Legacy Shaders/Transparent/Cutout/Diffuse" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200

CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert

sampler2D _MainTex;
fixed4 _Color;

struct Input {
    float2 uv_MainTex;
	fixed4 my_vertColor;
};

void vert (inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input,o);
    o.my_vertColor=v.color;
}
		 
void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color*IN.my_vertColor;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
