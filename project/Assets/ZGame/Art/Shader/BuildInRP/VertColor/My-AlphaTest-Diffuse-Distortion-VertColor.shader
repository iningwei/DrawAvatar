//https://blog.csdn.net/weixin_30616969/article/details/95424748
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "My/VertColor/Legacy Shaders/Transparent/Cutout/Diffuse/Distortion" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	
	_DistortionTex("Distortion Texture",2D)="white" {}
	_DistortionSpeed("Distortion Speed", Range(0,3.0)) = 1.0
	_DistortionPower("Distortion Power",Range(0,3.0))=1.0
}

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200


CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert

sampler2D _MainTex;
fixed4 _Color;

sampler2D _DistortionTex;
float _DistortionSpeed;
float _DistortionPower;

 
 
struct Input {
    float2 uv_MainTex;
	fixed4 my_vertColor;
	
	 float2 uv_DistortionTex;
};

void vert (inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input,o);
    o.my_vertColor=v.color;
	
	//o.uv_DistortionTex.x-=(_Time.y*_DistortionSpeed);//为啥不生效呢？？？。在surf中算的话比较耗性能
	 
}
		 
void surf (Input IN, inout SurfaceOutput o) {     
	IN.uv_DistortionTex.x-=(_Time.y*_DistortionSpeed);
	fixed4 distortionCol=tex2D(_DistortionTex,IN.uv_DistortionTex);
	half2 distortionUV = {distortionCol.r,distortionCol.g};
    //颜色值是(0,1)，而扰动需要(-1,1)
    distortionUV = (distortionUV-0.5)*2;
	
	 fixed4 c = tex2D(_MainTex, IN.uv_MainTex+distortionUV*0.02*_DistortionPower);
	 c=c* _Color*IN.my_vertColor;
	 o.Albedo = c.rgb;
     o.Alpha = c.a;
    
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
