using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompSpriteRendererInfo : BuildInCompInfo, IFillCompElement
    {
        public SpriteRenderer concreteCompSpriteRenderer;
        public List<SpriteInfo> refSprites;
        public BuildInCompSpriteRendererInfo(Transform tran, SpriteRenderer refSpriteRenderer, List<SpriteInfo> refSprites, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompSpriteRenderer = refSpriteRenderer;
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
                    if (this.concreteCompSpriteRenderer.material != null)
                    {
                        oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(this.concreteCompSpriteRenderer.material.name);
                        if (oldMatRes != null)
                        {
                            oldMatName = oldMatRes.resName;
                        }
                    }
                    mat = matRes.GetResAsset<Material>();
                    this.concreteCompSpriteRenderer.material = mat;

                    //remove mat ref
                    if (oldMatRes != null && oldMatName != matRes.resName)
                    {
                        oldMatRes.RemoveRefTrs(this.tran);
                    }
                    //add mat ref  
                    matRes.AddRefTrs(this.tran);
                }

                //fill sprites
                if (this.refSprites != null && this.refSprites.Count > 0)
                {
                    ABManager.Instance.LoadSpriteToTarget<SpriteRenderer>(this.concreteCompSpriteRenderer, this.refSprites[0].atlasName, this.refSprites[0].spriteName, sync);
                }


                ABManager.Instance.ResetEditorShader(tran, mat, shaderName);
            }
        }
    }
}