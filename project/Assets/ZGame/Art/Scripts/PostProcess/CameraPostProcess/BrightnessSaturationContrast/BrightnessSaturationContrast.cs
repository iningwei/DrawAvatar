using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.PostProcess.CameraPostProcess
{
    [ExecuteInEditMode]
    public class BrightnessSaturationContrast : PostEffectBase
    {
        public float brightness = 1f;
        public float saturation = 1f;
        public float contrast = 1f;

        public override void Start()
        {
            base.Start();
            shaderName = "My/PostProcess/BrightnessSaturationContrast";

        }
        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (mat)
            {
                mat.SetFloat("_Brightness", brightness);
                mat.SetFloat("_Saturation", saturation);
                mat.SetFloat("_Contrast", contrast);

                Graphics.Blit(src, dst, mat);
            }
            else
            {
                Graphics.Blit(src, dst);
            }
        }
    }
}