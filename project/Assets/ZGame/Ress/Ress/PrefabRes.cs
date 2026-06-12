using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress
{
    [Serializable]
    public class PrefabRes : Res
    {

        public PrefabRes(string name, GameObject obj) : base(name, obj)
        {

        }

        public override T GetResAsset<T>()
        {
            T result = default(T);

            if (!(typeof(T).Equals(typeof(GameObject))))
            {
                Debug.LogError("类型不匹配GameObject");
            }


            result = (T)(object)resObj;
            if (result == null)
            {
                Debug.LogError("Get PrefabRes fail :" + this.resName);
            }
            return result;

        }
    }
}
