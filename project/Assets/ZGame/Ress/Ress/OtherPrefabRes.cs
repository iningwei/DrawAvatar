using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [Serializable]
    public class OtherPrefabRes : PrefabRes
    {
        public OtherPrefabRes(string name, GameObject obj) : base(name, obj)
        {

        }

        public override T GetResAsset<T>()
        {
            return base.GetResAsset<T>();
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.LogError("remove otherprefab res:" + resName);

            ABManager.Instance.RemoveCachedRes(ABType.OtherPrefab, this);
        }
    }
}
