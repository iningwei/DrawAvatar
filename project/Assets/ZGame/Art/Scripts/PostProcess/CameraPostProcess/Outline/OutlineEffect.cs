using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.PostProcess.CameraPostProcess
{
    //参考：https://willweissman.wordpress.com/tutorials/shaders/unity-shaderlab-object-outlines/
    public class OutlineEffect : PostEffectBase
    {
        Camera attachedCam;
        Camera tmpCam;

        private Shader _outline;
        private Material _mat;

        public Shader outline
        {
            get
            {
                if (_outline == null)
                {
                    _outline = Shader.Find("My/PostProcess/Outline");
                }
                return _outline;
            }
        }
        public Material mat
        {
            get
            {

                _mat = CheckShaderAndCreateMaterial(outline, _mat);
                return _mat;
            }
        }

        private void Start()
        {
            attachedCam = this.GetComponent<Camera>();

            tmpCam = new GameObject().AddComponent<Camera>();
            tmpCam.name = "tmpCam";
            tmpCam.enabled = false;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            tmpCam.CopyFrom(attachedCam);
            tmpCam.clearFlags = CameraClearFlags.Color;
            tmpCam.backgroundColor = Color.black;

            //tmpCam.cullingMask = 1 << LayerMask.NameToLayer("Outline");

            RenderTexture tmpRt = new RenderTexture(src.width, src.height, 0, RenderTextureFormat.R8);
            tmpRt.Create();

            tmpCam.targetTexture = tmpRt;
            tmpCam.Render();

            Graphics.Blit(tmpRt, dst,mat);
            tmpRt.Release();
        }
    }
}