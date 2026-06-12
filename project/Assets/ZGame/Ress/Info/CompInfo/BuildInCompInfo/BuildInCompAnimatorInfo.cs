using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompAnimatorInfo : CompInfo, IFillCompElement
    {
        public Animator concreteCompAnimator;
        public string refAnimatorControllerName;

        public BuildInCompAnimatorInfo(Transform tran, Animator refAnimator, string refAnimatorControllerName) : base(tran)
        {
            this.concreteCompAnimator = refAnimator;
            this.refAnimatorControllerName = refAnimatorControllerName;
        }

        public void FillCompElement(bool sync)
        {
            if (!string.IsNullOrEmpty(this.refAnimatorControllerName))
            {
                AnimatorControllerRes oldACRes = null;
                string oldACName = "";
                AnimatorControllerRes acRes = null;
                RuntimeAnimatorController ac = null;
                ABManager.Instance.LoadAnimatorController(this.refAnimatorControllerName, (res) =>
                {
                    acRes = res;
                    if (acRes != null)
                    {
                        ac = res.GetResAsset<RuntimeAnimatorController>();

                        if (this.concreteCompAnimator.runtimeAnimatorController != null)
                        {
                            oldACRes = ABManager.Instance.GetCachedRes<AnimatorControllerRes>(this.concreteCompAnimator.runtimeAnimatorController.name);
                            if (oldACRes != null)
                            {
                                oldACName = oldACRes.resName;
                            }
                        }

                        this.concreteCompAnimator.runtimeAnimatorController = ac;
                        if (this.concreteCompAnimator.enabled)
                        {
                            this.concreteCompAnimator.enabled = false;
                            this.concreteCompAnimator.enabled = true;
                        }//avoid animator not play default anim. 

                        //remove ac ref
                        if (oldACRes != null && oldACName != acRes.resName)
                        {
                            oldACRes.RemoveRefTrs(this.tran);
                        }
                        //add ac ref
                        acRes.AddRefTrs(this.tran);

                        //TODO:加载动画片段clip 
                    }


                }, sync);
            }
        }

    }
}