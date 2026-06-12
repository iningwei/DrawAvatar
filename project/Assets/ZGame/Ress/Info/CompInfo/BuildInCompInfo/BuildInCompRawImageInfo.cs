using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompRawImageInfo : BuildInCompInfo, IFillCompElement
    {
        public RawImage concreteCompRawImage;
        public string texName;
        public BuildInCompRawImageInfo(Transform tran, RawImage refRawImg, string texName, string meshName, int matIndex, string matName, string shaderName) : base(tran, meshName, matIndex, matName, shaderName)
        {
            this.concreteCompRawImage = refRawImg;
            this.texName = texName;
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
                    if (this.concreteCompRawImage.material != null)
                    {
                        oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(this.concreteCompRawImage.material.name);
                        if (oldMatRes != null)
                        {
                            oldMatName = oldMatRes.resName;
                        }
                    }
                    mat = matRes.GetResAsset<Material>();
                    this.concreteCompRawImage.material = mat;

                    //remove mat ref
                    if (oldMatRes != null && oldMatName != matRes.resName)
                    {
                        oldMatRes.RemoveRefTrs(this.tran);
                    }
                    //add mat ref  
                    matRes.AddRefTrs(this.tran);
                }

                //fill texture
                if (!string.IsNullOrEmpty(this.texName))
                {
                    ABManager.Instance.LoadTextureToRawImage(concreteCompRawImage, texName, sync);
                }

                ABManager.Instance.ResetEditorShader(tran, mat, shaderName);
            }
        }
    }
}