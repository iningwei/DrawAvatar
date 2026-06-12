using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABObject
    {
        /// <summary>
        /// name:no prefix,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, Action<ObjectRes> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                var obj = objs[0];

                ObjectRes res = new ObjectRes(name, obj);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                callback?.Invoke(res);
            };

            AB.Load(name, ABType.Object, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}