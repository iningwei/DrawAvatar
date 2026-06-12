using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.PostProcess.CameraPostProcess
{
    public class DepthMapEffect : PostEffectBase
    {



        public override void Start()
        {
            base.Start();
            shaderName = "My/PostProcess/DepthMap";
            //向前渲染模式下，需要如下设置，才能获得场景的深度信息
            //若渲染模式为延迟渲染，则不需再设置
            this.attachedCam.depthTextureMode = DepthTextureMode.Depth;

        }
        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, mat);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

}