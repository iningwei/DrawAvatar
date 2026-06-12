using System;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABPrefab
    {
        /// <summary>
        /// name:no prefix ,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, ABType abType, Action<UnityEngine.Object> callback, bool sync)
        { 
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
        {
            //as for prefab, objs[0] is still a prefab,so you should instantiate it to GameObject
            GameObject entityObj = GameObject.Instantiate(objs[0]) as GameObject;


            Res res = null;
            if (abType == ABType.Effect)
            {
                res = new EffectRes(name, entityObj);
            }
            else if (abType == ABType.Window)
            {
                res = new WindowRes(name, entityObj);
            }
            else if (abType == ABType.OtherPrefab)
            {
                res = new OtherPrefabRes(name, entityObj);

            }
            //TODO:other type

            EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

            if (callback != null)
            {
                callback(entityObj);
            }
        };

            if (abType != ABType.Effect && abType != ABType.Window && abType != ABType.OtherPrefab)
            {
                DebugExt.LogE("ABPrefab do not support Load " + abType.ToString());
            }

            AB.Load(name, abType, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}