using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;


//一些自定义二进制 bytes 文件
namespace ZGame.Ress
{
    [Serializable]
    public class ByteRes : Res
    {
        string name;
        TextAsset asset;


        public ByteRes(string name, TextAsset resObj) : base(name, resObj)
        {
            this.name = this.resName;
            this.asset = resObj;
        }

        public override T GetResAsset<T>()
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(TextAsset))))
            {
                Debug.LogError("类型不匹配 TextAsset");
            }
            result = (T)(object)asset;


            if (result == null)
            {
                Debug.LogError("error, get res fail, asset is null");
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
