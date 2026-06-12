using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame.PostProcess.CameraPostProcess
{
    //暂时还不知道啥用
    //source:https://forum.unity.com/threads/apply-post-processing-image-effects-to-specific-camera-layer.141870/
    //Image effects apply always to the current content on screen, which includes any pixels rendered with another camera before.
//However, some effects like "glow" are using the alpha channel as mask.So all you have to do is clear the alpha channel, either by not writing it in the first place or by clearing it afterwards.

//Clearing the alpha could be achieved by using the following image effect(on your first camera) :
    public class ClearAlpha : PostEffectBase
    {
        private Shader _shader;

        public Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Shader.Find("My/PostProcess/ClearAlpha");
                }
                return _shader;
            }
        }


        static Material m_Material = null;
        protected Material material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = new Material(shader);
                    m_Material.hideFlags = HideFlags.HideAndDontSave;
                    m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_Material;
            }
        }


        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}