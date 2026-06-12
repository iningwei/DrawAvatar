using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABAnimationClip
    {
        public static void Load(string name, Action<AnimationClipRes> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                AnimationClip clip = objs[0] as AnimationClip;
                if (clip == null)
                {
                    DebugExt.LogE("load AnimationClip " + name + " null");
                }


                AnimationClipRes res = new AnimationClipRes(name, clip);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                if (callback != null)
                {
                    callback(res);
                }
            };


            AB.Load(name, ABType.AnimationClip, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}