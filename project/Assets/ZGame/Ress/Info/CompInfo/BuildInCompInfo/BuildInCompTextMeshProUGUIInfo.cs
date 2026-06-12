using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompTextMeshProUGUIInfo : BuildInCompInfo, IFillCompElement
    {
        public TextMeshProUGUI concreteCompTextMeshProUGUI;
        public BuildInCompTextMeshProUGUIInfo(Transform tran, TextMeshProUGUI refTextMeshProUGUI, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompTextMeshProUGUI = refTextMeshProUGUI;
        }

        public void FillCompElement(bool sync)
        {
            //////if (this.matIndex != -1)
            //////{
            //////    MatRes oldMatRes = null;
            //////    string oldMatName = "";
            //////    MatRes matRes = null;
            //////    Material mat = null;
            //////    ABManager.Instance.LoadMat(this.matName, (res) =>
            //////    {
            //////        matRes = res;
            //////    });
            //////    //set mat and handle mat ref
            //////    if (matRes != null)
            //////    {
            //////        if (this.concreteCompTextMeshProUGUI.material != null)
            //////        {
            //////            oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(this.concreteCompTextMeshProUGUI.material.name);
            //////            if (oldMatRes != null)
            //////            {
            //////                oldMatName = oldMatRes.resName;
            //////            }
            //////        }
            //////        mat = matRes.GetResAsset<Material>();
            //////        this.concreteCompTextMeshProUGUI.material = mat;

            //////        //remove mat ref
            //////        if (oldMatRes != null && oldMatName != matRes.resName)
            //////        {
            //////            oldMatRes.RemoveRefTrs(this.tran);
            //////        }
            //////        //add mat ref  
            //////        matRes.AddRefTrs(this.tran);
            //////    }

            //////    ABManager.Instance.ResetEditorShader(tran, mat, shaderName);
            //////}
            ///

            ABManager.Instance.ResetEditorShader(tran, concreteCompTextMeshProUGUI.fontSharedMaterial, shaderName);
        }
    }
}