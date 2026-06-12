using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [System.Serializable]
    public class AnimatorControllerRes : Res
    {
        string name;
        public AnimatorControllerRes(string name, UnityEngine.Object resObj) : base(name, resObj)
        {
            this.name = resName;

        }

        public override T GetResAsset<T>()
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(RuntimeAnimatorController))))
            {
                Debug.LogError("类型不匹配 RuntimeAnimatorController");
            }

            result = (T)(object)this.resObj;

            if (result == null)
            {
                Debug.LogError("error, get res fail,RuntimeAnimatorController is null， RuntimeAnimatorControllerName:" + this.resName);
            }

            return result;
        }

        public override void Destroy()
        {
            base.Destroy();

            ABManager.Instance.RemoveCachedRes(ABType.AnimatorController, this);
        }
    }
}