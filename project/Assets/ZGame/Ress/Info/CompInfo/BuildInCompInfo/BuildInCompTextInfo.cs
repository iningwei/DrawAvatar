using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompTextInfo : BuildInCompInfo,IFillCompElement
    {
        public Text concreteCompText;
        public BuildInCompTextInfo(Transform tran, Text refText, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompText = refText;
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
                    if (this.concreteCompText.material != null)
                    {
                        oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(this.concreteCompText.material.name);
                        if (oldMatRes != null)
                        {
                            oldMatName = oldMatRes.resName;
                        }
                    }
                    mat = matRes.GetResAsset<Material>();
                    this.concreteCompText.material = mat;

                    //remove mat ref
                    if (oldMatRes != null && oldMatName != matRes.resName)
                    {
                        oldMatRes.RemoveRefTrs(this.tran);
                    }
                    //add mat ref  
                    matRes.AddRefTrs(this.tran);
                }
                 
                ABManager.Instance.ResetEditorShader(tran, mat, shaderName);
            }
        }
    }
}