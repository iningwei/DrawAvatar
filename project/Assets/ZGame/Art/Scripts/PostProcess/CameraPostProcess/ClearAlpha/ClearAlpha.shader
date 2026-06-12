Shader "My/PostProcess/ClearAlpha"
{
 
    SubShader {
        ColorMask A
        ZTest Always Cull Off ZWrite Off Fog { Mode Off }
       Pass { Color (0,0,0,0) }
    }
    Fallback off
 
}
