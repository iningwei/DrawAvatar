using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;
namespace ZGame.Ress.AB
{
    public class ABMesh : MonoBehaviour
    {
        /// <summary>
        /// name:no prefix,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, Action<MeshRes> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                Mesh mesh = objs[0] as Mesh;
                if (mesh == null)
                {
                    DebugExt.LogE("load mesh " + name + " null");
                }


                MeshRes res = new MeshRes(name, mesh);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                if (callback != null)
                {
                    callback(res);
                }
            };



            AB.Load(name, ABType.Mesh, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);

        }
    }
}