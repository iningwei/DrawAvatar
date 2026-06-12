using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABAnimatorController
    {
        /// <summary>
        /// name:no prefix,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, Action<AnimatorControllerRes> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                var obj = objs[0];

                AnimatorControllerRes res = new AnimatorControllerRes(name, obj);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                callback?.Invoke(res);
            };

            AB.Load(name, ABType.AnimatorController, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}