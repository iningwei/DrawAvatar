using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABByte
    {
        /// <summary>
        /// name:no prefix,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, Action<TextAsset> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {

                TextAsset asset = objs[0] as TextAsset;

                if (asset == null)
                {
                    Debug.LogError("load byte " + name + " null");
                } 

                ByteRes res = new ByteRes(name, asset);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                if (callback != null)
                {
                    callback(asset);
                }
            };

            AB.Load(name, ABType.Byte, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}