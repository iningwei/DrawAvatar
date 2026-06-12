 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
namespace ZGame.Ress.Info
{
    //////[System.Serializable]
    //////public class ExtCompShaderMeshAnimationInfo : ExtCompInfo, IFillCompElement
    //////{
    //////    public ShaderMeshAnimator concreteCompSMA;
    //////    public string defaultAnimation;
    //////    public List<string> animations;

    //////    public ExtCompShaderMeshAnimationInfo(Transform tran, ShaderMeshAnimator refSMA, string defaultAnim, List<string> anims) : base(tran)
    //////    {
    //////        this.concreteCompSMA = refSMA;
    //////        this.defaultAnimation = defaultAnim;
    //////        this.animations = anims;
    //////    }

    //////    public void FillCompElement(bool sync)
    //////    {

    //////        if (!string.IsNullOrEmpty(this.defaultAnimation) && this.animations.Count > 0)
    //////        {

    //////            //defaultAnimation
    //////            ABManager.Instance.LoadObject(this.defaultAnimation, (objRes) =>
    //////              {
    //////                  if (objRes != null)
    //////                  {
    //////                      var obj = objRes.GetResAsset<UnityEngine.Object>();
    //////                      this.concreteCompSMA.defaultMeshAnimation = obj as ShaderMeshAnimation;

    //////                      //TODO:remove ref
    //////                      //add ShaderMeshAnim ref  
    //////                      objRes.AddRefTrs(this.tran);
    //////                  }
    //////              }, sync);
    //////            //animations
    //////            for (int i = 0; i < this.animations.Count; i++)
    //////            {
    //////                ABManager.Instance.LoadObject(this.animations[i], (objRes) =>
    //////                {
    //////                    if (objRes != null)
    //////                    {
    //////                        var obj = objRes.GetResAsset<UnityEngine.Object>();
    //////                        this.concreteCompSMA.meshAnimations[i] = obj as ShaderMeshAnimation;
    //////                        //TODO:remove ref
    //////                        //add ShaderMeshAnim ref  
    //////                        objRes.AddRefTrs(this.tran);
    //////                    }
    //////                }, sync);
    //////            }
    //////        }
    //////    }
    //////}
}