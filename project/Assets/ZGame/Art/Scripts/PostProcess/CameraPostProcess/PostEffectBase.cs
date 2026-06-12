using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//较新版本的Unity中已经标示SystemInfo.supportsImageEffects、SystemInfo.supportsRenderTextures ，always return true。
//因此不需要再判定平台支持与否。

namespace ZGame.PostProcess.CameraPostProcess
{

    [RequireComponent(typeof(Camera))]
    public class PostEffectBase : MonoBehaviour
    {
        public Camera attachedCam;
        public string shaderName;

        private Shader _shader;
        private Material _mat;

        public Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Shader.Find(shaderName);
                }
                if (_shader == null)
                {
                    DebugExt.LogE("can not find shader with name:" + shaderName);
                }
                return _shader;
            }
        }
        public Material mat
        {
            get
            {
                _mat = CheckShaderAndCreateMaterial(shader, _mat);
                return _mat;
            }
        }

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m)
        {
            if (s == null || !s.isSupported)
            {
                if (!s.isSupported)
                {
                    DebugExt.LogE(s.name + " is not support on this device");
                }
                return null;
            }

            if (s.isSupported && m && m.shader == s)
            {
                return m;
            }
            m = new Material(s);
            m.hideFlags = HideFlags.DontSave;
            return m ? m : null;
        }


        public virtual void Start()
        {
            attachedCam = this.GetComponent<Camera>();
            if (attachedCam == null)
            {
                DebugExt.LogE("error,no camera attached to:" + this.gameObject.GetHierarchy());
            }
        }


        public virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
        {

        }

        public virtual void OnDestroy()
        {
            DebugExt.Log("PostEffectBase OnDestroy");
            if (_mat != null)
            {
                GameObject.DestroyImmediate(_mat);
            }
        }
    }
}