using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [System.Serializable]
    public class TextureRes : ImgRes
    {
        string name;
        Texture tex;

        public TextureRes(string name, Texture resObj) : base(name, resObj)
        {
            this.name = this.resName;
            this.tex = resObj;
        }


        public override T GetResAsset<T>()
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(Texture))))
            {
                Debug.LogError("类型不匹配Texture");
            }

            result = (T)(object)this.tex;

            if (result == null)
            {
                Debug.LogError("error, get res fail,texture is null， textureName:" + name);
            }

            return result;
        }


        public override void Destroy()
        {
            base.Destroy();


            ABManager.Instance.RemoveCachedRes(ABType.Texture, this);
        }
    }

}