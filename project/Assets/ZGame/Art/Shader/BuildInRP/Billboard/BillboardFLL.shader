// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 
//冯乐乐版本
Shader "My/Billboard/BillboardFLL"
{
    Properties
    {
      _MainTex ("Base texture", 2D) = "white" {}
	 _VerticalBillboarding("Vertical Billboard restrict",Range(-2,2))=0
    }
	
	
   SubShader { 
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  "DisableBatching" = "True" }
	
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	Lighting Off 
	ZWrite Off 
	
	LOD 100
	
	CGINCLUDE	
	#include "UnityCG.cginc"
 
	sampler2D _MainTex;
	 float4 _MainTex_ST;
	 float _VerticalBillboarding;

	struct appdata {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };
		 
	struct v2f {
		float4	vertex	: SV_POSITION;
		float2	texcoord	: TEXCOORD0;
	};
 
 
	
	
	v2f vert (appdata v)
	{
		v2f o;
		 
		float3 center = float3(0, 0, 0); //物体空间原点
		//将相机位置转换至物体空间并计算相对原点朝向，物体旋转后的法向将与之平行，这里实现的是Viewpoint-oriented Billboard
		float3 viewer = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos, 1));
		float3 normalDir = viewer - center;
		// _VerticalBillboarding为0到1，控制物体法线朝向向上的限制，实现Axial Billboard到World-Oriented Billboard的变换
		normalDir.y =normalDir.y * _VerticalBillboarding;
		normalDir = normalize(normalDir);
		//若原物体法线已经朝向上，这up为z轴正方向，否者默认为y轴正方向
		float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
		//利用初步的upDir计算righDir，并以此重建准确的upDir，达到新建转向后坐标系的目的
		float3 rightDir = normalize(cross(upDir, normalDir));     
		upDir = normalize(cross(normalDir, rightDir));
		// 计算原物体各顶点相对位移，并加到新的坐标系上
		float3 centerOffs = v.vertex.xyz - center;
		float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
		o.vertex = UnityObjectToClipPos(float4(localPos, 1));
	 
		o.texcoord =TRANSFORM_TEX( v.texcoord,_MainTex);
		return o;
	}
	ENDCG
 
 
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		 
		fixed4 frag (v2f i) : COLOR
		{		
		fixed4 col = tex2D(_MainTex, i.texcoord);
			return col;
		}
		ENDCG 
	}	
} 

}
