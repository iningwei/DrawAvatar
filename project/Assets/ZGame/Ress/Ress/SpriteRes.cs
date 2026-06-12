using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZGame.Ress.AB;
using ZGame.Event;

namespace ZGame.Ress
{

    /// <summary>
    /// sprite atlas
    /// especially means atlas created by  texturepacker or similar tools!
    /// </summary>
    [Serializable]
    public class SpriteRes : ImgRes
    {
        [SerializeField]
        string atlasName;
        [SerializeField]
        public Sprite[] allSprites;

        public SpriteRes(string name, Texture2D resObj, Sprite[] allSprites) : base(name, resObj)
        {
            this.atlasName = this.resName;
            this.allSprites = allSprites;

        }



        public override T GetResAsset<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(Sprite))))
            {
                Debug.LogError("type not equal to Sprite");
            }
            else
            {
                for (int i = 0; i < allSprites.Length; i++)
                {
                    if (allSprites[i].name == name)
                    {
                        result = (T)((object)allSprites[i]);
                        break;
                    }
                }
            }
            if (result == null)
            {
                Debug.LogError("GetRes fail, atlasName:" + this.atlasName + " has no sprite with name:" + name);
            }

            return result;
        }


        public override void Destroy()
        {
            base.Destroy();

            //unload all sprite assets
            if (allSprites != null)
            {
                for (int i = allSprites.Length - 1; i >= 0; i--)
                {
                    Resources.UnloadAsset(allSprites[i]);
                }
                allSprites = null;
            }
            //TODO:texture,mesh等等实体也要删除

            ABManager.Instance.RemoveCachedRes(ABType.Sprite, this);
        }
    }
}