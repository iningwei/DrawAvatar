using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [System.Serializable]
    public class MatRes : Res
    {
        string name;
        Material mat;
        public MatRes(string name, Material resObj) : base(name, resObj)
        {
            this.name = this.resName;
            this.mat = resObj;
        }
        public override T GetResAsset<T>()
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(Material))))
            {
                Debug.LogError("类型不匹配 Material");
            }

            result = (T)(object)this.mat;

            if (result == null)
            {
                Debug.LogError("error, get res fail,mat is null， matName:" + this.resName);
            }

            return result;
        }

        public override void Destroy()
        {
            base.Destroy();

            ABManager.Instance.RemoveCachedRes(ABType.Material, this);
        }
    }
}
