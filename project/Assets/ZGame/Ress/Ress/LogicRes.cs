using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress
{
    public class LogicRes : Res
    {
        string name;
        TextAsset textAsset;

        public LogicRes(string name, TextAsset resObj) : base(name, resObj)
        {
            this.name = this.resName;
            textAsset = resObj;
        }

        public override T GetResAsset<T>( )
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(TextAsset))))
            {
                Debug.LogError("类型不匹配 TextAsset");
            }
             

            result = (T)(object)this.textAsset;

            if (result == null)
            {
                Debug.LogError("error, get res fail,textAsset is null");
            }

            return result;
        }
    }
}
