using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame.PostProcess.CameraPostProcess
{
    //当前相机渲染目标灰化（指的是针对当前相机渲染的Layer进行灰化，不影响比它更早渲染的相机结果）
    //通过简单设置tmpCam的CullingMask可以随意对目标Layer进行灰化
    public class GrayEffect : PostEffectBase
    {
        Camera tmpCam;
        RenderTexture tmpRT;
        public override void Start()
        {
            base.Start();

            shaderName = "My/PostProcess/Gray";

            attachedCam = this.GetComponent<Camera>();
            tmpCam = new GameObject().AddComponent<Camera>();
            tmpCam.transform.parent = attachedCam.transform;
            tmpCam.transform.localPosition = Vector3.zero;
            tmpCam.gameObject.name = "TmpCamForGrayEff";

            tmpCam.enabled = false;

            tmpCam.CopyFrom(attachedCam);
            //tmpCam.cullingMask = 1 << LayerMask.NameToLayer("Scene");

            //attachedCam.cullingMask = 0;//设置当前相机不再渲染东西
            //一般情况下可以不用设置。shader中已经做了判断处理。况且设置了相机不渲染东西，势必会影响该相机下的点击事件等其它逻辑功能的使用。
            tmpRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {

            //put it to video memory
            tmpRT.Create();
            tmpRT.name = "grayEffectTmpRt";

            tmpCam.targetTexture = tmpRT;
            tmpCam.Render();

            //tmpCam.RenderWithShader(gray, "");

            //混合src和tmpRT,并对tmpRT灰化
          
            if (mat)
            {
                mat.SetTexture("_SceneTex", src);
                Graphics.Blit(tmpRT, dst, mat);
            }
            else
            {
                Graphics.Blit(tmpRT, dst);
            }

            tmpRT.Release();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (tmpRT != null)
            {
                tmpRT.Release();
            }
        }

    }
}