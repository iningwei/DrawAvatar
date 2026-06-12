// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// based Legacy Shaders/Particles/Additive
// made distortion shader for weapon trail
Shader "My/WeaponTrail/Additive-Distortion" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    _DistortionTex("Distortion Texture",2D)="white" {}
	_DistortionSpeed("Distortion Speed", Range(0,3.0)) = 1.0
	_DistortionPower("Distortion Power",Range(0,3.0))=1.0
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha One
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;            
            fixed4 _TintColor;
			sampler2D _DistortionTex;
			float _DistortionSpeed;
			float _DistortionPower;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 distortion_uv:TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 distortion_uv:TEXCOORD3;
                UNITY_FOG_COORDS(1)
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;
            float4 _DistortionTex_ST;


            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.distortion_uv = TRANSFORM_TEX(v.texcoord,_DistortionTex);
                o.distortion_uv.x-=(_Time.y*_DistortionSpeed);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            float _InvFade;

            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                i.color.a *= fade;
                #endif



                fixed4 distortionCol = tex2D(_DistortionTex, i.distortion_uv);
                half2 distortionUV = {distortionCol.r,distortionCol.g};
                //颜色值是(0,1)，而扰动需要(-1,1)
                distortionUV = (distortionUV-0.5)*2;
                fixed4 col = tex2D(_MainTex, i.texcoord + distortionUV*0.02*_DistortionPower);


                //fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
                col = 2.0f * i.color * _TintColor * col;


                col.a = saturate(col.a); // alpha should not have double-brightness applied to it, but we can't fix that legacy behavior without breaking everyone's effects, so instead clamp the output to get sensible HDR behavior (case 967476)

                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
                return col;
            }
            ENDCG
        }
    }
}
}
