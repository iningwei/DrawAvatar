using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;


//一些不容易归类，但是基类是UnityEngine.Object类型的资源
namespace ZGame.Ress
{
    [Serializable]
    public class ObjectRes : Res
    {
        string name;
        UnityEngine.Object asset;


        public ObjectRes(string name, UnityEngine.Object resObj) : base(name, resObj)
        {
            this.name = this.resName;
            this.asset = resObj;
        }

        public override T GetResAsset<T>()
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(UnityEngine.Object))))
            {
                Debug.LogError("类型不匹配 UnityEngine.Object");
            }


            result = (T)(object)asset;


            if (result == null)
            {
                Debug.LogError("error, get res fail, asset is null:" + this.resName);
            }
            return result;
        }

        public override void Destroy()
        {
            base.Destroy();
            ABManager.Instance.RemoveCachedRes(ABType.Byte, this);
        }
    }



}
