using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [Serializable]
    public class AnimationClipRes : Res
    {
        string name;
        AnimationClip clip;
        public AnimationClipRes(string name, AnimationClip resObj) : base(name, resObj)
        {
            this.name = resName;
            this.clip = resObj;
        }

        public override T GetResAsset<T>()
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(AnimationClip))))
            {
                Debug.LogError("类型不匹配 AnimationClip");
            }

            result = (T)(object)clip;
            if (result == null)
            {
                Debug.LogError("error, get res fail, AnimationClip is null");
            }
            return result;
        }

        public override void Destroy()
        {
            base.Destroy();
            //////ABManager.Instance.RemoveCachedRes(ABType.AnimationClip, this);//本项目常驻
        }
    }
}