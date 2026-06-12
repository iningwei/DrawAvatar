using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class SpriteInfo : ImgInfo
    {
        /// <summary>
        /// 图集名
        /// </summary>
        public string atlasName;
        /// <summary>
        /// 精灵名
        /// </summary>
        public string spriteName;

        public SpriteInfo(string atlasName, string spriteName)
        {
            this.atlasName = atlasName;
            this.spriteName = spriteName;
        }
    }
}