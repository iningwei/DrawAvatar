using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompRendererInfo : BuildInCompInfo, IFillCompElement
    {
        public Renderer concreteCompRenderer;
        public List<TextureInfo> refTextures;
        public BuildInCompRendererInfo(Transform tran, Renderer refRenderer, List<TextureInfo> refSprites, string meshName, int matIndex, string matName, string shaderName) : base(tran, meshName, matIndex, matName, shaderName)
        {
            this.concreteCompRenderer = refRenderer;
            this.refTextures = refSprites;
        }

        public void FillCompElement(bool sync)
        {
            if (this.meshName != "")
            {
                MeshRes oldMeshRes = null;
                string oldMeshName = "";
                MeshRes meshRes = null;
                Mesh mesh = null;
                ABManager.Instance.LoadMesh(this.meshName, (res) =>
                {
                    meshRes = res;
                    if (meshRes != null)
                    {
                        mesh = res.GetResAsset<Mesh>();
                        if (this.concreteCompRenderer is MeshRenderer)
                        {
                            var curMesh = this.tran.GetComponent<MeshFilter>().sharedMesh;
                            if (curMesh != null)
                            {
                                oldMeshRes = ABManager.Instance.GetCachedRes<MeshRes>(curMesh.name);
                                if (oldMeshRes != null)
                                {
                                    oldMeshName = oldMeshRes.resName;
                                }
                            }

                            this.tran.GetComponent<MeshFilter>().sharedMesh = mesh;


                        }
                        else if (this.concreteCompRenderer is SkinnedMeshRenderer)
                        {
                            var curMesh = (this.concreteCompRenderer as SkinnedMeshRenderer).sharedMesh;
                            if (curMesh != null)
                            {
                                oldMeshRes = ABManager.Instance.GetCachedRes<MeshRes>(curMesh.name);
                                if (oldMeshRes != null)
                                {
                                    oldMeshName = oldMeshRes.resName;
                                }
                            }

                            (this.concreteCompRenderer as SkinnedMeshRenderer).sharedMesh = mesh;
                        }
                        else if (this.concreteCompRenderer is ParticleSystemRenderer)
                        {
                            var curMesh = (this.concreteCompRenderer as ParticleSystemRenderer).mesh;
                            if (curMesh != null)
                            {
                                oldMeshRes = ABManager.Instance.GetCachedRes<MeshRes>(curMesh.name);
                                if (oldMeshRes != null)
                                {
                                    oldMeshName = oldMeshRes.resName;
                                }
                            }
                             (this.concreteCompRenderer as ParticleSystemRenderer).mesh = mesh;
                        }


                        //remove mesh ref
                        if (oldMeshRes != null && oldMeshName != meshRes.resName)
                        {
                            oldMeshRes.RemoveRefTrs(this.tran);
                        }
                        //add mesh ref 
                        meshRes.AddRefTrs(this.tran);
                    }

                }, sync);

            }

            if (this.matIndex != -1)
            {
                MatRes oldMatRes = null;
                string oldMatName = "";
                MatRes matRes = null;
                Material mat = null;
                if (this.matName.ToLower() == "default-material")
                {
                    Debug.LogWarning("used default-material,please check!!!" + this.tran.GetHierarchy());
                }
                ABManager.Instance.LoadMat(this.matName, (res) =>
                {
                    matRes = res;
                });//cur mat only support sync load,so we can use like following

                //set mat and handle mat ref
                if (matRes != null)
                {
                    mat = matRes.GetResAsset<Material>();

                    //////if (this.concreteCompRenderer is ParticleSystemRenderer)
                    //////{
                    //////    var curMat = this.concreteCompRenderer.sharedMaterial;
                    //////    if (curMat != null)
                    //////    {
                    //////        oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(curMat.name);
                    //////        if (oldMatRes != null)
                    //////        {
                    //////            oldMatName = oldMatRes.resName;
                    //////        }
                    //////    }
                    //////    this.concreteCompRenderer.sharedMaterial = mat; 
                    //////}
                    //////else
                    //////{
                    Material[] sharedMaterialsCopy = this.concreteCompRenderer.sharedMaterials;
                    if (sharedMaterialsCopy[this.matIndex] != null)
                    {
                        oldMatRes = ABManager.Instance.GetCachedRes<MatRes>(sharedMaterialsCopy[this.matIndex].name);
                        if (oldMatRes != null)
                        {
                            oldMatName = oldMatRes.resName;
                        }
                    }

                    sharedMaterialsCopy[this.matIndex] = mat;
                    this.concreteCompRenderer.sharedMaterials = sharedMaterialsCopy;
                    //////}
                    //这里不用区分ParticleSystemRenderer，并对其特殊处理
                    //在Inspector界面中看ParticleSystemRenderer确实没有materials概念，在Renderer下最多只有Material和Trail Material(如果勾选了Trails)。
                    //但是ParticleSystemRenderer的material搜集在 BuildInCompRendererCollection.cs中也是通过sharedMaterials进行搜集的，且断点看过是包含了Material和Trail Material的。
                    //因此在反向设置时，依然通过sharedMaterials的方式进行设置


                    //remove mat ref
                    if (oldMatRes != null && oldMatName != matRes.resName)
                    {
                        oldMatRes.RemoveRefTrs(this.tran);
                    }
                    //add mat ref  
                    matRes.AddRefTrs(this.tran);
                }

                if (matRes != null)
                {
                    //fill texture
                    if (this.refTextures != null && this.refTextures.Count > 0)
                    {
                        for (int k = 0; k < this.refTextures.Count; k++)
                        {
                            ABManager.Instance.LoadTextureToMat(this.tran, this.refTextures[k].texName, this.refTextures[k].shaderProp, mat, sync, null);
                        }
                    }


                    ABManager.Instance.ResetEditorShader(tran, mat, shaderName);
                }

            }
        }
    }
}