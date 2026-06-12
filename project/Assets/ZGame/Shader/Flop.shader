//·­ÅÆÐ§¹û
Shader "Custom/Flop" {
Properties {
_Color ("Color", Color) = (1,1,1,1)
_MainTex ("Albedo (RGB)", 2D) = "white" {}
_Glossiness ("Smoothness", Range(0,1)) = 0.5
_Metallic ("Metallic", Range(0,1)) = 0.0
_MyDis("Dis",Range(0,20)) =0
}
SubShader {
Tags { "RenderType"="Opaque" }
LOD 200
cull off
CGPROGRAM
// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows vertex:vert

// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

sampler2D _MainTex;

struct Input {
float2 uv_MainTex;
};

half _Glossiness;
half _Metallic;
float _MyDis;
fixed4 _Color;

void vert(inout appdata_full v) {
half x = v.vertex.x;
half y = v.vertex.y;
half z = v.vertex.z;
half3 p0 = half3(0, 0, z);
half3 p1 = half3(7.5, 0, z);
half3 p2 = half3(5-_MyDis*0.2, 0+ _MyDis*0.5, z);
if (v.vertex.x > 0) {
half t = (v.vertex.x) / 10;
t = clamp(t, 0, 1);
half3 end = (1 - t)*(1 - t)*p0 + 2 * t*(1 - t)*p1 + t*t*p2;
v.vertex = half4(end, 0);// mul(mt, v.vertex);
}
}
void surf (Input IN, inout SurfaceOutputStandard o) {
// Albedo comes from a texture tinted by color
fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
o.Albedo = c.rgb;
// Metallic and smoothness come from slider variables
o.Metallic = _Metallic;
o.Smoothness = _Glossiness;
o.Alpha = c.a;
}
ENDCG
}
FallBack "Diffuse"
}


/*

half4x4 mt = {
1,0,0,end.x,
0,1,0,end.y,
0,0,1,end.z,
0,0,0,1
};
*/