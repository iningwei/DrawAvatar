using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;
using UnityEngine.SceneManagement;

namespace ZGame.Ress
{
    public class SceneRes : Res
    {
        public SceneRes(string name, Object resObj) : base(name, resObj)
        {
        }

        public override T GetResAsset<T>()
        {
            T result = default(T);

            if (!(typeof(T).Equals(typeof(Scene))))
            {
                Debug.LogError("类型不匹配 Scene");
            }


            result = (T)(object)resObj;
            if (result == null)
            {
                Debug.LogError("Get SceneRes fail :" + this.resName);
            }
            return result;

        }
    }
}