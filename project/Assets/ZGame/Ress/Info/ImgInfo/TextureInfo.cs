using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class TextureInfo : ImgInfo
    {

        /// <summary>
        /// tex名
        /// </summary>
        public string texName;

        /// <summary>
        /// shader属性
        /// </summary>
        public string shaderProp;

        public TextureInfo(string texName, string shaderProp)
        {
            this.texName = texName;
            this.shaderProp = shaderProp;
        }
    }

}