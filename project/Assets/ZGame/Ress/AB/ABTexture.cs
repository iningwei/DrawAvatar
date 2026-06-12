using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABTexture
    {
        /// <summary>
        /// texName: no suffix and prefix
        /// </summary>
        /// <param texName="texName">texName为图片名称</param>
        /// <returns></returns>
        public static void Load(string texName, Action<Texture> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                Texture tex = objs[0] as Texture;

                TextureRes res = new TextureRes(texName, tex);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                if (callback != null)
                {
                    callback(tex);
                }
            };

            AB.Load(texName, ABType.Texture, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}