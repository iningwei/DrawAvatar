using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.AB;
using ZGame.UGUIExtention;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    /// <summary>
    /// 内置组件Image
    /// Image承载的图片资源类型是Sprite，不支持对Texture的直接使用
    /// </summary>
    public class BuildInCompImageInfo : BuildInCompInfo, IFillCompElement
    {
        public Image concreteCompImage;
        public List<SpriteInfo> refSprites;

        public BuildInCompImageInfo(Transform tran, Image refImage, List<SpriteInfo> refSprites, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompImage = refImage;
            this.refSprites = refSprites;
        }

        public void FillCompElement(bool sync)
        {
            if (this.matIndex != -1)
            {
                MatRes oldMatRes = null;
                string oldMatName = "";
                MatRes matRes = null;
                Material mat = null;
                ABManager.Instance.LoadMat(this.matName, (res) =>
                 {
                     matRes = res;
                 });
                //set mat and handle mat ref
                if (matRes != null)
                {
                    if (this.concreteCompImage.material != null)
                    {
                        oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(this.concreteCompImage.material.name);
                        if (oldMatRes != null)
                        {
                            oldMatName = oldMatRes.resName;
                        }
                    }
                    mat = matRes.GetResAsset<Material>();
                    this.concreteCompImage.material = mat;

                    //remove mat ref
                    if (oldMatRes != null && oldMatName != matRes.resName)
                    {
                        oldMatRes.RemoveRefTrs(this.tran);
                    }
                    //add mat ref  
                    matRes.AddRefTrs(this.tran);
                }

                //fill sprite
                if (this.refSprites != null && this.refSprites.Count > 0)
                {
                    ABManager.Instance.LoadSpriteToTarget<Image>(concreteCompImage, refSprites[0].atlasName, refSprites[0].spriteName, sync);
                }

                ABManager.Instance.ResetEditorShader(tran, mat, shaderName);
            }
        }
    }
}