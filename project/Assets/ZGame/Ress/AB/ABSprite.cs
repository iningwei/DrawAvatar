using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABSprite
    {

        public static void Load(string atlasName, string spriteName, Action<Sprite> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                Texture2D tex2D = objs[0] as Texture2D;
                int spriteCount = objs.Length - 1;
                Sprite[] childSprites = new Sprite[spriteCount];
                for (int i = 0; i < spriteCount; i++)
                {
                    childSprites[i] = objs[i + 1] as Sprite;
                }



                Sprite target = null;
                for (int i = 0; i < childSprites.Length; i++)
                {
                    if (childSprites[i].name == spriteName)
                    {
                        target = childSprites[i];
                        break;
                    }
                }

                if (target == null)
                {
                    DebugExt.LogE("atlasName:" + atlasName + " have no sprite:" + spriteName);
                }

                SpriteRes res = new SpriteRes(atlasName, tex2D, childSprites);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                if (callback != null)
                {
                    callback(target);
                }
            };

            AB.Load(atlasName, ABType.Sprite, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }


}