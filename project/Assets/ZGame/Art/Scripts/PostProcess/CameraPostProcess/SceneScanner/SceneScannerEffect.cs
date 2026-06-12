using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.PostProcess.CameraPostProcess
{

    /// <summary>
    /// 场景内物体扫描效果
    /// 游戏效果参考：无人深空
    /// </summary>
    public class SceneScannerEffect : PostEffectBase
    {
        public float velocity = 0.5f;
        bool isScanning = false;
        float dis;

        public override void Start()
        {
            base.Start();
            shaderName = "My/PostProcess/SceneScanner";
            attachedCam.depthTextureMode = DepthTextureMode.Depth;
        }

        private void Update()
        {
            if (this.isScanning)
            {
                this.dis += Time.deltaTime * this.velocity;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                this.isScanning = true;
                this.dis = 0;
            }
        }
        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            mat.SetFloat("_ScanDistance", dis);
            Graphics.Blit(src, dst, mat);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}